using System;
using System.Collections.Generic;
using Possan.MapReduce.IO;

namespace Possan.MapReduce.Util
{
	public class Splitter
	{
		class NonSortingPartitioningWriter : IRecordWriter<string, string>
		{
			private readonly IList<IFileDestination<string, string>> _outputs;
			private readonly IShardingPartitioner _partitioner;
			private Dictionary<int, IRecordWriter<string, string>> _partitionwriters;
			private int _numshards;

			public NonSortingPartitioningWriter(IList<IFileDestination<string, string>> outputs, IShardingPartitioner partitioner)
			{
				_numshards = outputs.Count;
				_outputs = outputs;
				_partitioner = partitioner;
				_partitionwriters = new Dictionary<int, IRecordWriter<string, string>>();
			}

			public void Dispose()
			{
				lock (_partitionwriters)
				{
					int n = 0;
					var ks = _partitionwriters.Keys;
					int t = ks.Count;
					foreach (var k in ks)
					{
						if (n % 500 == 0)
							Console.WriteLine("Finishing partition writer " + n + " / " + t);
						n++;
						_partitionwriters[k].Dispose();
					}
				}
			}

			public void Write(string key, string value)
			{
				// Console.WriteLine("Splitter.PartitioningWriter Writing " + key + ", value " + value);

				var p = _partitioner.Partition(key, _numshards);
				if (!_partitionwriters.ContainsKey(p))
					lock (_partitionwriters)
					{
						if (!_partitionwriters.ContainsKey(p))
							_partitionwriters.Add(p, _outputs[p].CreateWriter());
					}
				_partitionwriters[p].Write(key, value);
			}

			public void Write(IRecordReader<string, string> reader)
			{
				throw new NotImplementedException();
			}

			public void Write(IRecordStreamReader<string, string> reader)
			{
				throw new NotImplementedException();
			}
		}

		class SortingPartitioningWriter : IRecordWriter<string, string>
		{
			private readonly IList<IFileDestination<string, string>> _outputs;
			private readonly IShardingPartitioner _partitioner;
			private Dictionary<int, NonLockingMemoryKeyValueReaderWriter> _partitionwriters;
			private int _numshards;

			public SortingPartitioningWriter(IList<IFileDestination<string, string>> outputs, IShardingPartitioner partitioner)
			{
				_numshards = outputs.Count;
				_outputs = outputs;
				_partitioner = partitioner;
				_partitionwriters = new Dictionary<int, NonLockingMemoryKeyValueReaderWriter>();
			}

			public void Dispose()
			{
				lock (_partitionwriters)
				{
					var ks = _partitionwriters.Keys;
					foreach (var pk in ks)
					{
						Console.WriteLine("Finishing partition writer " + pk);
						if (_partitionwriters[pk].Count == 0)
							continue;

						var keys = _partitionwriters[pk].GetKeys();
						var w = _outputs[pk].CreateWriter();
						foreach (var k in keys)
						{
							var values = _partitionwriters[pk].GetValues(k);
							foreach (var v in values)
								w.Write(k, v);
						}
						w.Dispose();
						_partitionwriters[pk].Dispose();
					}
				}
			}

			public void Write(string key, string value)
			{
				// Console.WriteLine("Splitter.PartitioningWriter Writing " + key + ", value " + value);

				var p = _partitioner.Partition(key, _numshards);
				if (!_partitionwriters.ContainsKey(p))
					lock (_partitionwriters)
					{
						if (!_partitionwriters.ContainsKey(p))
							_partitionwriters.Add(p, new NonLockingMemoryKeyValueReaderWriter());
					}
				_partitionwriters[p].Write(key, value);
			}

			public void Write(IRecordReader<string, string> reader)
			{
				throw new NotImplementedException();
			}

			public void Write(IRecordStreamReader<string, string> reader)
			{
				throw new NotImplementedException();
			}
		}

		class FileSplitterThread : PooledThread
		{
			public IRecordWriter<string, string> outputwriter;
			public string FileId;
			public IFileSource<string, string> filesource;

			override public void InnerRun()
			{
				using (var rdr = filesource.CreateStreamReader(FileId))
				{
					string k, v;
					while (rdr.Read(out k, out v))
					{
						outputwriter.Write(k, v);
					}
				}
			}
		}

		public static void Split(IList<IFileSource<string, string>> inputs, IList<IFileDestination<string, string>> outputs, IShardingPartitioner partitioner, bool sortOutput)
		{
			var t0 = new Timing("Splitting");

			IRecordWriter<string, string> writer;
			if (sortOutput)
				writer = new SortingPartitioningWriter(outputs, partitioner);
			else
				writer = new NonSortingPartitioningWriter(outputs, partitioner);

			var stp = new ThreadPool(10);
			foreach (var input in inputs)
			{
				Console.WriteLine("Preparing input partitioning.");
				string inputfile;
				while (input.ReadNext(out inputfile))
				{
					Console.WriteLine("Splitter found file " + inputfile);
					stp.Queue(new FileSplitterThread
					{
						outputwriter = writer,
						filesource = input,
						FileId = inputfile
					});
					stp.Step();
				}
			}
			Console.WriteLine("Partitioning input files for mappers...");
			stp.WaitAll();

			writer.Dispose();

			Console.WriteLine("Partitioning done.");
			t0.End();

			GC.Collect();
		}
	}
}