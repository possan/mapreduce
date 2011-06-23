using System;

namespace Possan.MapReduce.Util
{
	public class Combiner
	{
		class CombinerThread : PooledThread
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
						outputwriter.Write(k, v);
				}
			}
		}

		public static void Combine(IFileSource<string, string> inputs, IFileDestination<string, string> output)
		{
			var t0 = new Timing("Combining files");
			using (var writer = output.CreateWriter())
			{
				var stp = new ThreadPool(5);
				Console.WriteLine("Preparing combiners...");
				string inputfile;
				while (inputs.ReadNext(out inputfile))
				{
					stp.Queue(new CombinerThread
					          	{
					          		outputwriter = writer,
					          		filesource = inputs,
					          		FileId = inputfile
					          	});
				}
				Console.WriteLine("Combining files...");
				stp.WaitAll();
			}
			t0.End();
			Console.WriteLine("Combining done.");
			GC.Collect();
		}
	}
}