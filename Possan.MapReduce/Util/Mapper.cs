using System;
using System.Collections.Generic;
using Possan.MapReduce.IO;

namespace Possan.MapReduce.Util
{
	public class Mapper
	{
		class MapFilesThread : PooledThread
		{
			class MapAndReduceThread : PooledThread
			{
				public IFileDestination<string, string> OutputFolder = null;
				public string inputkey = "";
				public string value = "";
				public IMapper mapper = null;
				public IReducer reducer = null;

				override public void InnerRun()
				{
					var tmp = new NonLockingMemoryKeyValueReaderWriter();
					try
					{
						mapper.Map(inputkey, value, new RecordWriterMapperCollector(tmp));
					}
					catch (Exception e)
					{
						Console.WriteLine("Mapper crashed: " + e);
					}

					if (tmp.Count == 0)
						return;

					if (reducer != null)
					{
						var tmp2 = new NonLockingMemoryKeyValueReaderWriter();
					
						var rw = new RecordWriterReducerCollector(tmp2);
						foreach (var k in tmp.GetKeys())
						{
							try
							{
								reducer.Reduce(k, tmp.GetValues(k), rw, false);
							}
							catch (Exception e)
							{
								Console.WriteLine("Reducer crashed: " + e);
							}
						}

						using (var w = OutputFolder.CreateWriter())
						{
							w.Write(tmp2);
						}
					}
					else
					{
						using (var w = OutputFolder.CreateWriter())
						{
							w.Write(tmp);
						}
					}
				}
			}

			public IMapper mapper;
			public IReducer reducer;
			public IFileSource<string, string> InputFolders;
			public IFileDestination<string, string> OutputFolder;
			public List<string> FileIDs;
			public ThreadPool threadpool;

			public MapFilesThread()
			{
				FileIDs = new List<string>();
			}

			public override void InnerRun()
			{
				int kidx = 0;
				foreach (var fileid in FileIDs)
				{
					using (var rdr = InputFolders.CreateStreamReader(fileid))
					{
						string k, v;
						while (rdr.Read(out k, out v))
						{
							if (kidx % 1000 == 0 && kidx > 0)
								Console.WriteLine("Queueing mapper " + kidx + " (" + fileid + ") ...");
							threadpool.Queue(new MapAndReduceThread
							{
								mapper = mapper,
								reducer = reducer,
								OutputFolder = OutputFolder,
								inputkey = k,
								value = v
							});
							kidx++;
						}
					}
				}
			}
		}

		public static void Map(IFileSource<string, string> inputfolders, IFileDestination<string, string> shuffleroutput, IMapper mapper)
		{
			Map(inputfolders, shuffleroutput, mapper, null, 50);
		}

		public static void Map(IFileSource<string, string> inputfolders, IFileDestination<string, string> shuffleroutput, IMapper mapper, IReducer prereducer)
		{
			Map(inputfolders, shuffleroutput, mapper, prereducer, 50);
		}

		public static void Map(IFileSource<string, string> inputfolders, IFileDestination<string, string> shuffleroutput, IMapper mapper, int threads)
		{
			Map(inputfolders, shuffleroutput, mapper, null, threads);
		}

		public static void Map(IFileSource<string, string> inputfolders, IFileDestination<string, string> shuffleroutput, IMapper mapper, IReducer prereducer, int threads)
		{
			// var partitioner = new StandardKeyPartitioner(Count);

			var threadpool = new ThreadPool(threads, "Mapper file threads");
			var threadpool2 = new ThreadPool(threads, "Mapper threads");

			Console.WriteLine("Preparing mappers...");

			string inputfileid;
			while (inputfolders.ReadNext(out inputfileid))
			{
				var ft = new MapFilesThread();
				ft.InputFolders = inputfolders;
				ft.OutputFolder = shuffleroutput;
				ft.mapper = mapper;
				ft.reducer = prereducer;
				ft.FileIDs.Add(inputfileid);
				ft.threadpool = threadpool2;
				threadpool.Queue(ft);
				threadpool.Step();
			}

			Console.WriteLine("Waiting for mapper file threads to finish...");
			var t1 = new Timing("Inner mapper");
			threadpool.WaitAll();
			t1.End();
			Console.WriteLine("Mapper file threads finished.");

			Console.WriteLine("Waiting for mapper threads to finish...");
			var t2 = new Timing("Inner mapper");
			threadpool2.WaitAll();
			t2.End();
			Console.WriteLine("Mapper threads finished.");

			GC.Collect();
		}
	}
}
