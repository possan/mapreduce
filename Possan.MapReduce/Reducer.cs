using System;
using System.Collections.Generic;

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
				_writer.Write(key, value);
			}
		}

		class ReducerThread : PooledThread
		{
			public string key;
			public IReducer reducer;
			public IRecordReader<string, string> input;
			public IRecordWriter<string, string> output;

			override public void InnerRun()
			{
				var values2 = new List<string>(input.GetValues(key));
				if (values2.Count > 100)
					Console.WriteLine("Reducing " + values2.Count + " values for key " + key);
				try
				{
					reducer.Reduce(key, values2, new ReducerCollector(output));
				}
				catch (Exception e)
				{
					Console.WriteLine("Reducer crashed: " + e);
				}
				// Console.WriteLine("Reduce of key "+key+" done.");
			}
		}

		public static void Reduce(IRecordReader<string, string> input, IRecordWriter<string, string> output, IReducer reducer)
		{
			var threadpool = new ThreadPool(60);
			var keys = new List<string>(input.GetKeys());
			for (var k = 0; k < keys.Count; k++)
			{
				if (k % 100 == 0 && k > 0)
					Console.WriteLine("Queueing reducer " + k + "/" + keys.Count + "...");

				var key = keys[k];
				var rt = new ReducerThread();
				rt.reducer = reducer;
				rt.key = key;
				rt.output = output;
				rt.input = input;
				threadpool.Queue(rt);
			}
			Console.WriteLine("Waiting for threads to finish.");
			threadpool.WaitAll();
			Console.WriteLine("Threads finished.");
		}
	}
}