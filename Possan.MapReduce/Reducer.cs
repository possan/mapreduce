using System;
using System.Collections.Generic;
using Possan.MapReduce.Impl;

namespace Possan.MapReduce
{
	public class Reducer
	{
		class ReducerCollector : IReducerCollector
		{
			private readonly IRecordWriter<string, string> _writer;

			public ReducerCollector(IRecordWriter<string, string> writer)
			{
				_writer = writer;
			}

			public void Collect(string key, string value)
			{
				// Console.WriteLine("collected key=" + key + ", value=" + value);
				_writer.Write(key, value);
			}
		}

		class ReducerThread : PooledThread
		{
			public IReducer reducer;
			public IStorage storage;
			public string inputbatch;
			public string targetbatch;
			public string key;

			override public void InnerRun()
			{
				// Console.WriteLine("Starting reducer for " + key);
				var targetwriter = new RecordWriter(storage, targetbatch);
				var values = new List<string>();
				var keybatch = inputbatch + "/" + key;
				var valuefiles = new List<string>(storage.GetAllFilesInBatch(keybatch));
				for (int i = 0; i < valuefiles.Count; i++)
				{
					if (i % 100 == 0 && i != 0)
						Console.WriteLine("Loading values " + i + " of " + valuefiles.Count + " (" + valuefiles[i] + ")");
					var f = valuefiles[i];
					values.Add(storage.Get(keybatch, f));
				}
				if (values.Count > 100)
					Console.WriteLine("Writing " + values.Count + " values to key " + key);
				var collector = new ReducerCollector(targetwriter);
				try
				{
					reducer.Reduce(key, values, collector);
				}
				catch (Exception e)
				{
					Console.WriteLine("Reducer crashed: " + e);
				} 
			}
		}

		public static IList<string> Reduce(IStorage storage, string[] inputbatches, string targetbatch, IReducer reducer)
		{
			var threadpool = new ThreadPool(40);
			// var threads = new List<ReducerThread>();
			foreach (var inputbatch in inputbatches)
			{
				Console.WriteLine("Reducing from batch " + inputbatch);
				var keys = new List<string>(storage.GetSubBatches(inputbatch));
				for (var k = 0; k < keys.Count; k++)
				{
					var key = keys[k];
					var rt = new ReducerThread();
					rt.reducer = reducer;
					rt.key = key;
					rt.storage = storage;
					rt.targetbatch = targetbatch;
					rt.inputbatch = inputbatch;
					threadpool.Queue(rt);
				}
			}

			Console.WriteLine("Waiting for threads to finish.");
			threadpool.WaitAll();
			Console.WriteLine("Threads finished.");

			var ret = new List<string>();
			ret.Add(targetbatch);
			return ret;
		}
	}
}