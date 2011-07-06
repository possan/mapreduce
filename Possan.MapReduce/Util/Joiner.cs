using System;
using System.Collections.Generic;

namespace Possan.MapReduce.Util
{
	public class Joiner
	{
		class PartitioningWriter : IRecordWriter<string, string>
		{
			private readonly IFileDestination<string, string> _output;
			private readonly IPartitioner _partitioner;
			private Dictionary<string, IRecordWriter<string, string>> _partitionwriters;

			public PartitioningWriter(IFileDestination<string, string> output, IPartitioner partitioner)
			{
				_output = output;
				_partitioner = partitioner;
				_partitionwriters = new Dictionary<string, IRecordWriter<string, string>>();
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
				var p = _partitioner.Partition(key);
				if (!_partitionwriters.ContainsKey(p))
					lock (_partitionwriters)
					{
						if (!_partitionwriters.ContainsKey(p))
							_partitionwriters.Add(p, _output.CreateWriter());
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

		public static void Join(IFileSource<string, string> inputs, IFileDestination<string, string> output, IPartitioner partitioner)
		{
			var t0 = new Timing("Joining");
			using (var writer = new PartitioningWriter(output, partitioner))
			{
				var stp = new ThreadPool(10);
				Console.WriteLine("Preparing input partitioning.");
				string inputfile;
				while (inputs.ReadNext(out inputfile))
				{
					// Console.WriteLine("Splitter found file " + inputfile);
					stp.Queue(new FileSplitterThread { outputwriter = writer, filesource = inputs, FileId = inputfile });
					stp.Step();
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