using System;
using System.Collections.Generic;

namespace Possan.MapReduce.Util
{
	public class Splitter
	{
		class PartitioningWriter : IRecordWriter<string, string>
		{
			private readonly IList<IFileDestination<string, string>> _outputs;
			private readonly IShardingPartitioner _partitioner;
			private Dictionary<int, IRecordWriter<string, string>> _partitionwriters;
			private int _numshards;


			public PartitioningWriter(IList<IFileDestination<string, string>> outputs, IShardingPartitioner partitioner)
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
						// Console.WriteLine("splitter found: " + k + " => " + v);
						outputwriter.Write(k, v);
					}
				}
			}
		}

		public static void Split(IList<IFileSource<string, string>> inputs, IList<IFileDestination<string, string>> outputs, IShardingPartitioner partitioner)
		{
			var t0 = new Timing("Splitting");

			using (var writer = new PartitioningWriter(outputs, partitioner))
			{
				var stp = new ThreadPool(10);
				foreach (var input in inputs)
				{
					Console.WriteLine("Preparing input partitioning.");
					string inputfile;
					while (input.ReadNext(out inputfile))
					{
						Console.WriteLine("Splitter found file " + inputfile);
						stp.Queue(new FileSplitterThread { outputwriter = writer, filesource = input, FileId = inputfile });
						stp.Step();
					}

				}
				Console.WriteLine("Partitioning input files for mappers...");
				stp.WaitAll();
			}
			Console.WriteLine("Partitioning done.");
			t0.End();

			GC.Collect();
		}
	}
}