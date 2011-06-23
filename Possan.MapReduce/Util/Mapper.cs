using System;
using System.Collections.Generic;
using Possan.MapReduce.IO;

namespace Possan.MapReduce.Util
{
	public class Mapper
	{
		class MapFilesThread : PooledThread
		{
			class MapThread : PooledThread
			{
				public IMapperCollector collector;
				public string inputkey;
				public string value;
				public IMapper mapper;

				override public void InnerRun()
				{
					try
					{
						mapper.Map(inputkey, value, collector);
					}
					catch (Exception e)
					{
						Console.WriteLine("Mapper crashed: " + e);
					}
				}
			}

			class MapAndReduceThread : PooledThread
			{
				public IFileDestination<string, string> OutputFolder;
				public string inputkey;
				public string value;
				public IMapper mapper;
				public IReducer reducer;

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

					if( tmp.Count == 0 )
						return;

					var w = OutputFolder.CreateWriter();
					var rw = new RecordWriterReducerCollector(w);
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
					w.Dispose();
				}
			}

			public IMapper mapper;
			public IReducer reducer;
			public IFileSource<string, string> InputFolders;
			public IFileDestination<string, string> OutputFolder;
			public List<string> FileIDs;

			public MapFilesThread()
			{
				FileIDs = new List<string>();
			}

			public override void InnerRun()
			{
				if( reducer != null )
				{
					var threadpool = new ThreadPool(50, "Single file mapper and reducer");
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

								var mt = new MapAndReduceThread();
								mt.mapper = mapper;
								mt.reducer = reducer;
								mt.OutputFolder = OutputFolder;
								mt.inputkey = k;
								mt.value = v;
								threadpool.Queue(mt);
								threadpool.Step();
								kidx++;
							}
						}
					}

					// Console.WriteLine("Waiting for mappers to finish.");
					var t1 = new Timing("Inner mapper for " + FileIDs.Count + " files.");
					threadpool.WaitAll();
					t1.End();
				}
				else
				{

					var threadpool = new ThreadPool(50, "Single file mapper");
					int kidx = 0;
					using (var coll = new FileDestinationMapperCollector { Output = OutputFolder })
					{
						foreach (var fileid in FileIDs)
						{
							using (var rdr = InputFolders.CreateStreamReader(fileid))
							{
								string k, v;
								while (rdr.Read(out k, out v))
								{
									if (kidx % 1000 == 0 && kidx > 0)
										Console.WriteLine("Queueing mapper " + kidx + " (" + fileid + ") ...");

									var mt = new MapThread();
									mt.mapper = mapper;
									mt.collector = coll;
									mt.inputkey = k;
									mt.value = v;
									threadpool.Queue(mt);
									threadpool.Step();
									kidx++;
								}
							}
						}

						// Console.WriteLine("Waiting for mappers to finish.");
						var t1 = new Timing("Inner mapper for " + FileIDs.Count + " files.");
						threadpool.WaitAll();
						t1.End();
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

			Console.WriteLine("Preparing mappers...");

			string inputfileid = "";
			int counter = 0;
			MapFilesThread ft = null;
			while (inputfolders.ReadNext(out inputfileid))
			{
				if (ft == null)
				{
					ft = new MapFilesThread();
					ft.InputFolders = inputfolders;
					ft.OutputFolder = shuffleroutput;
					ft.mapper = mapper;
					ft.reducer = prereducer;
				}

				ft.FileIDs.Add(inputfileid);

				counter++;
				if (counter > 2)
				{
					counter = 0;
					threadpool.Queue(ft);
					ft = null;
				}
			}

			if (ft != null)
			{
				threadpool.Queue(ft);
				ft = null;
			}

			Console.WriteLine("Waiting for mapper threads to finish...");
			var t1 = new Timing("Inner mapper");
			threadpool.WaitAll();
			t1.End();
			Console.WriteLine("Mapper threads finished.");

			GC.Collect();
		}
	}
}
