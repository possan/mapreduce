using System;
using System.Collections.Generic;
using System.Threading;

namespace Possan.MapReduce.Util
{
	public class ThreadPool
	{
		private List<IPooledThread> m_running;
		private Queue<IPooledThread> m_notstarted;
		private List<IPooledThread> m_done;

		public static int MaximumTotalConcurrentThreads = 2 * Environment.ProcessorCount;

		private int m_maxsimultaneous;
		private string m_name;
		private int _queuecount;

		public ThreadPool(int simultaneous, string name)
			: this(simultaneous)
		{
			m_name = name;
		}

		public ThreadPool(int simultaneous)
		{
			m_name = "";
			_queuecount = 0;
			m_running = new List<IPooledThread>();
			m_notstarted = new Queue<IPooledThread>();
			m_done = new List<IPooledThread>();
			m_maxsimultaneous = simultaneous;
			if (m_maxsimultaneous > MaximumTotalConcurrentThreads)
				m_maxsimultaneous = MaximumTotalConcurrentThreads;
		}


		public void Queue(IPooledThread thread)
		{
			lock (m_running)
			{
				thread.Thread = new Thread(thread.Run);
				m_notstarted.Enqueue(thread);
				_queuecount++;
				if (_queuecount % 1000 == 0 && _queuecount > 0)
					Console.WriteLine("Threadpool queued " + _queuecount + " jobs...");
			}
		}

		public void Step()
		{
			if (AnyRunning)
			{
				Monitor.Enter(m_running);
				for (int i = m_running.Count - 1; i >= 0; i--)
				{
					var item = m_running[i];
					var test = false;
					try
					{
						test = item.Signal.WaitOne(1, true);
					}
					catch (Exception z)
					{
					}
					if (test)
					{
						item.Signal.Close();
						m_running.RemoveAt(i);
						m_done.Add(item);
					}
				}
				Monitor.Exit(m_running);
			}

			while (AnyNotStarted && EmptySpace)
			{
				Monitor.Enter(m_running);
				var startme = m_notstarted.Dequeue();
				startme.Signal = new ManualResetEvent(false);
				m_running.Add(startme);
				startme.Thread.Start();
				Monitor.Exit(m_running);
			}
		}

		bool AnyRunning { get { return m_running.Count > 0; } }
		bool EmptySpace { get { return m_running.Count < m_maxsimultaneous; } }
		bool AnyNotStarted { get { return m_notstarted.Count > 0; } }

		public void WaitAll()
		{
			var lastnotst = m_notstarted.Count;
			var nextdump = DateTime.Now.AddSeconds(1);
			do
			{
				Step();
				if (DateTime.Now > nextdump)
				{
					float eta = 0;
					var diff = lastnotst - m_notstarted.Count;
					if (diff > 0)
						eta = (float)m_notstarted.Count / (float)diff;
					Console.WriteLine("Threadpool \""
						+ m_name + "\" waiting... ["
						+ m_running.Count + " running (of " + m_maxsimultaneous + "), "
						+ m_notstarted.Count + " in queue, "
						+ m_done.Count + " done, eta: " + eta + "]");
					nextdump = DateTime.Now.AddSeconds(1);
					lastnotst = m_notstarted.Count;
				}
				Thread.Sleep(10);
			}
			while (AnyRunning || AnyNotStarted);
			Console.WriteLine("Threadpool WaitAll Done.");
		}
	}
}