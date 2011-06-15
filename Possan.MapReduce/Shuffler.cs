using System;
using System.Collections.Generic;
using System.Threading;
using Possan.MapReduce.Impl;

namespace Possan.MapReduce
{
	public class Shuffler
	{
		class KeyShufflerThread : PooledThread
		{
			public IStorage storage;
			public string targetbatch;
			public string sourcebatch;
			public string key;

			override public void InnerRun()
			{
				// Console.WriteLine("Starting key shufflethread; key=" + key + ", source=" + sourcebatch + ", target=" + targetbatch);
				var reader = new RecordReader(storage, sourcebatch);
				var writer = new RecordWriter(storage, targetbatch);
				var values = new List<string>(reader.GetValues(key));
				for (var j = 0; j < values.Count; j++)
				{
					if (j != 0 && j % 100 == 0)
						Console.WriteLine("Copying values " + j + " of " + values.Count);
					var value = values[j];
					writer.Write(key, value);
				}
			}
		}

		class ShufflerThread : PooledThread
		{
			public IStorage storage;
			public string targetbatch;
			public string sourcebatch;

			override public void InnerRun()
			{
				Console.WriteLine("Starting shufflethread; source=" + sourcebatch + ", target=" + targetbatch);
				var reader = new RecordReader(storage, sourcebatch);
				var keys = new List<string>(reader.GetKeys());
				Console.WriteLine("Got " + keys.Count + " keys");
				var localthreadpool = new ThreadPool(5);
				for (int k = 0; k < keys.Count; k++)
				{
					var key = keys[k];
					if (k % 100 == 0 && k > 0)
						Console.WriteLine("Queueing shuffle for key " + key + " (" + k + " of " + keys.Count + ")");
					var kt = new KeyShufflerThread();
					kt.key = key;
					kt.storage = storage;
					kt.sourcebatch = sourcebatch;
					kt.targetbatch = targetbatch;
					localthreadpool.Queue(kt);
				}
				localthreadpool.WaitAll();
			}
		}

		public static IList<string> Shuffle(IStorage storage, string[] sourcebatches, string targetbatch)
		{
			var threadpool = new ThreadPool(5);

			Console.WriteLine("Shuffling..");
			foreach (var sourcebatch in sourcebatches)
			{
				Console.WriteLine("Shuffling from " + sourcebatch + " to " + targetbatch);
				var st = new ShufflerThread();
				st.storage = storage;
				st.sourcebatch = sourcebatch;
				st.targetbatch = targetbatch;
				threadpool.Queue(st);
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