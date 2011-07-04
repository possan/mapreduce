using System;
using Possan.MapReduce.IO;

namespace Possan.MapReduce.Util
{

	public class Reducer
	{
		class ReduceFileThread : PooledThread
		{
			public IReducer reducer;
			public IFileSource<string, string> InputFolders;
			public string FileID;
			public IFileDestination<string, string> OutputFolder;

			public override void InnerRun()
			{
				// Console.WriteLine("Reducer for " + FileID);

				// buffer input

				var tmp = new NonLockingMemoryKeyValueReaderWriter();
				using (var rdr = InputFolders.CreateStreamReader(FileID))
				{
					tmp.Write(rdr);
				}

				// Console.WriteLine(tmp.Count + " items found in " + FileID);

				// reduce

				var wrt = new NonLockingMemoryKeyValueReaderWriter();
				var coll = new RecordWriterReducerCollector(wrt);
				foreach (var kk in tmp.GetKeys())
				{
					// Console.WriteLine("Key "+kk+" has "+data[kk].Count+" values.");
					try
					{
						reducer.Reduce(kk, tmp.GetValues(kk), coll, true);
					}
					catch (Exception z)
					{
						Console.WriteLine("Reducer crashed: " + z);
					}
				}

				if (wrt.Count < 1)
					return;
				
				// save output

				using (var output = OutputFolder.CreateWriter())
				{
					output.Write(wrt);
				}
			}
		}

		public static void Reduce(IFileSource<string, string> inputfolders, IFileDestination<string, string> outputfolder, IReducer reducer)
		{
			Reduce(inputfolders, outputfolder, reducer, 30);
		}

		public static void Reduce(IFileSource<string, string> inputfolders, IFileDestination<string, string> outputfolder, IReducer reducer, int threads)
		{
			var threadpool = new ThreadPool(threads, "Reducer file threads");

			Console.WriteLine("Preparing reducer threads.");
			string inputfileid;
			while (inputfolders.ReadNext(out inputfileid))
			{
				var ft = new ReduceFileThread();
				ft.InputFolders = inputfolders;
				ft.OutputFolder = outputfolder;
				// 	ft.Writer = wrt;
				ft.reducer = reducer;
				ft.FileID = inputfileid;
				threadpool.Queue(ft);
			}

			Console.WriteLine("Waiting for reducer threads to finish...");
			var t1 = new Timing("Inner mapper");
			threadpool.WaitAll();
			t1.End();
			Console.WriteLine("Reducer threads finished.");

			GC.Collect();
		}
	}
}