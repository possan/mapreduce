using System;
using System.Collections.Generic;

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
			public IRecordWriter<string, string> outputwriter;
			public string inputkey;
			public string value;
			public IMapper mapper;

			override public void InnerRun()
			{
				var tempstorage = new NonLockingMemoryKeyValueReaderWriter();
				try
				{
					mapper.Map(inputkey, value, new MapperCollector(tempstorage));
				}
				catch (Exception e)
				{
					Console.WriteLine("Mapper crashed: " + e);
				}
				outputwriter.Write(tempstorage);
			}
		}

		public static void Map(IRecordReader<string, string> inputreader, IRecordWriter<string, string> outputwriter, IMapper mapper)
		{
			var threadpool = new ThreadPool(60);
			int kidx = 0;
			var keys = new List<string>(inputreader.GetKeys());
			for (int i = 0; i < keys.Count; i++)
			{
				var key = keys[i];
				if (i % 100 == 0 && i > 0)
					Console.WriteLine("Queueing mapper " + i + "/" + keys.Count + "...");

				var values = inputreader.GetValues(key);
				foreach (var value in values)
				{
					var mt = new MapperThread();
					mt.mapper = mapper;
					mt.outputwriter = outputwriter;
					mt.inputkey = key;
					mt.value = value;
					threadpool.Queue(mt);
				}
				kidx++;
			}
			Console.WriteLine("Waiting for threads to finish.");
			using (new Timing("Inner mapper"))
			{
				threadpool.WaitAll();
			}
			Console.WriteLine("Threads finished.");
		}
	}
}