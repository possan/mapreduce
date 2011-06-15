using System;
using System.Collections.Generic;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Possan.MapReduce.Impl
{
	public class MySqlStorage : IStorage, IDisposable
	{
		private readonly string _connectionstring;
		private readonly string _datatable;
		private Queue<ConnectionWrapper> pool;

		public MySqlStorage(string connectionstring, string tableprefix)
		{
			_connectionstring = connectionstring;
			_datatable = tableprefix + "data";
			pool = new Queue<ConnectionWrapper>();
		}

		public void CreateSchema()
		{
			// CREATE TABLE `mr`.`data` ( `batch` VARCHAR(200) NOT NULL , `key` VARCHAR(100) NOT NULL , `data` TEXT NOT NULL );
			// ALTER TABLE `mr`.`data` ADD INDEX `batchidx` (`batch` ASC), ADD INDEX `batchkeyidx` (`batch` ASC, `key` ASC) ;
		}

		public void ReleasePool()
		{
			while (pool.Count > 0)
			{
				var pc = pool.Dequeue();
				pc.connection.Clone();
			}
		}


		class ConnectionWrapper : IDisposable
		{
			public MySqlConnection connection;
			public MySqlDataReader lastreader;
			public MySqlStorage storage;

			public void Dispose()
			{
				storage.CloseConnection(this);
			}
		}

		private void CloseConnection(ConnectionWrapper wrapper)
		{
			if (wrapper.lastreader != null)
			{
				if (!wrapper.lastreader.IsClosed)
					wrapper.lastreader.Close();

				wrapper.lastreader = null;
			}

			// try to return to pool
			if (Monitor.TryEnter(pool, 100))
			{
				pool.Enqueue(wrapper);
				Monitor.Exit(pool);
			}
			else
			{
				wrapper.connection.Close();
			}
		}



		ConnectionWrapper OpenConnection()
		{
			ConnectionWrapper ret = null;

			// try to get from pool
			if (Monitor.TryEnter(pool, 100))
			{
				if (pool.Count > 0)
				{
					ret = pool.Dequeue();
					if (ret != null)
						ret.lastreader = null;
				}
				Monitor.Exit(pool);
			}

			if (ret == null)
			{
				// skapa ny connection
				ret = new ConnectionWrapper();
				ret.storage = this;
				ret.connection = new MySqlConnection(_connectionstring);
				ret.connection.Open();
			}

			return ret;
		}


		public string CreateFilename()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}

		public string CreateBatch()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}

		public void DeleteBatch(string batch)
		{
			using (var con = OpenConnection())
			{
				var cmd = con.connection.CreateCommand();
				cmd.CommandText = "delete from " + _datatable + " where b like @b";
				cmd.Parameters.AddWithValue("@b", batch + "/%");
				cmd.ExecuteNonQuery();
			}
		}

		public IEnumerable<string> GetSubBatches(string parentbatch)
		{
			var ret = new List<string>();
			using (var con = OpenConnection())
			{
				using (var cmd = con.connection.CreateCommand())
				{
					cmd.CommandText = "select b from " + _datatable + " where b like @b order by b asc";
					cmd.Parameters.AddWithValue("@b", parentbatch + "/%");
					using (var dr = cmd.ExecuteReader())
					{
						con.lastreader = dr;
						while (dr.Read())
						{
							var b = "";
							object o = dr["b"];
							if (o != null && !(o is DBNull))
							{
								b = o.ToString();
								b = b.Substring(parentbatch.Length + 1).Trim();

								var fls = b.IndexOf("/");
								if (fls != -1)
									b = b.Substring(0, fls);


								if (b != "")
									if (!ret.Contains(b))
										ret.Add(b);
							}
						}
						dr.Close();
						con.lastreader = null;
					}
				}
			}
			return ret;
		}

		public IEnumerable<string> GetAllFilesInBatch(string batch)
		{
			var ret = new List<string>();
			using (var con = OpenConnection())
			{
				using (var cmd = con.connection.CreateCommand())
				{
					cmd.CommandText = "select k from " + _datatable + " where b like @b";
					cmd.Parameters.AddWithValue("@b", batch + "/");
					using (var dr = cmd.ExecuteReader())
					{
						con.lastreader = dr;
						while (dr.Read())
						{
							object o = dr["k"];
							if (o != null && !(o is DBNull))
							{
								var fn = o.ToString();
								if (!ret.Contains(fn))
									ret.Add(fn);
							}
						}
						dr.Close();
						con.lastreader = null;
					}
				}
			}
			return ret;
		}

		public string Get(string batch, string filename)
		{
			var ret = "";
			using (var con = OpenConnection())
			{
				using (var cmd = con.connection.CreateCommand())
				{
					cmd.CommandText = "select d from " + _datatable + " where b=@b and k=@k";
					cmd.Parameters.AddWithValue("@b", batch + "/");
					cmd.Parameters.AddWithValue("@k", filename);
					using (var dr = cmd.ExecuteReader())
					{
						con.lastreader = dr;
						if (dr.Read())
						{
							object o = dr["d"];
							if (o != null && !(o is DBNull))
								ret = o.ToString();
						}
						dr.Close();
						con.lastreader = null;
					}
				}
			}
			return ret;
		}

		public void Put(string batch, string filename, string content)
		{
			using (var con = OpenConnection())
			{
				using (var cmd = con.connection.CreateCommand())
				{
					cmd.CommandText = "insert into " + _datatable + " (b,k,d) values ( @b,@k,@d );";
					cmd.Parameters.AddWithValue("@b", batch + "/");
					cmd.Parameters.AddWithValue("@k", filename);
					cmd.Parameters.AddWithValue("@d", content);
					cmd.ExecuteNonQuery();
				}
			}
		}

		public void Copy(string inputbatch, string inputfilename, string outputbatch, string outputfilename)
		{
			//	using (var con = OpenConnection())
			//	{
			// todo: snabba upp
			Put(outputbatch, outputfilename, Get(inputbatch, inputfilename));
			//	}
		}

		public void Dispose()
		{
			ReleasePool();
		}
	}
}
