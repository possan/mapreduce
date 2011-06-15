using System;
using System.Collections.Generic;
using Possan.MapReduce.Impl;

namespace Possan.MapReduce
{
	public class Mapper
	{
		class MapperCollector : IMapperCollector
		{
			private readonly IRecordWriter<string, string> _writer;

			public MapperCollector(IRecordWriter<string, string> writer)
			{
				_writer = writer;
			}

			public void Collect(string key, string value)
			{
				_writer.Write(key, value);
			}
		}

		class MapperThread : PooledThread
		{
			public IStorage storage;
			public string inputbatch;
			public string inputkey;
			public string outputbatch;
			public IMapper mapper;

			override public void InnerRun()
			{
				Console.WriteLine("Entered mapper thread for batch " + inputbatch + ", key " + inputkey);

				storage.DeleteBatch(outputbatch);

				var value = storage.Get(inputbatch, inputkey);
				var targetwriter = new RecordWriter(storage, outputbatch);
				var collector = new MapperCollector(targetwriter);
				try
				{
					mapper.Map(inputkey, value, collector);
				}
				catch (Exception e)
				{
					Console.WriteLine("Mapper crashed: " + e);
				}
				// Console.WriteLine("Exiting mapper thread for key " + inputkey);
			}
		}

		public static IList<string> Map(IStorage storage, string[] inputbatches, string outputbatch, IMapper mapper)
		{
			var outputbatches = new List<string>();
			var idx = 1;
			var threadpool = new ThreadPool(40);
			foreach (var inputbatch in inputbatches)
			{
				var files = storage.GetAllFilesInBatch(inputbatch);
				foreach (var f in files)
				{
					// TODO: run in parallel
					var targetfolder = outputbatch + "-" + idx; // storage.CreateBatch();// outputfolder + "\\mapper-" + idx;
					var mt = new MapperThread();
					mt.mapper = mapper;
					mt.storage = storage;
					mt.inputbatch = inputbatch;
					mt.outputbatch = targetfolder;
					mt.inputkey = f;
					threadpool.Queue(mt);
					outputbatches.Add(targetfolder);
					idx++;
				}
			}

			Console.WriteLine("Waiting for threads to finish.");
			threadpool.WaitAll();
			Console.WriteLine("Threads finished.");

			return outputbatches;
		}
	}
}