using System.Collections.Generic;
using System.Threading;

namespace Possan.MapReduce
{
	public interface IPooledThread
	{
		Thread Thread { get; set; }
		ManualResetEvent Signal { get; set; }
		void Run();
	}

	public class PooledThread : IPooledThread
	{
		public Thread Thread { get; set; }
		public ManualResetEvent Signal { get; set; }

		public void Run()
		{
			InnerRun();
			Signal.Set();
		}

		virtual public void InnerRun()
		{
		}
	}

	public class ThreadPool
	{
		private List<IPooledThread> m_running;
		private Queue<IPooledThread> m_notstarted;
		private List<IPooledThread> m_done;
		// private List<ManualResetEvent> m_events;

		public static int MaximumTotalConcurrentThreads = 60;

		private int m_maxsimultaneous;

		public ThreadPool(int simultaneous)
		{
			m_running = new List<IPooledThread>();
			m_notstarted = new Queue<IPooledThread>();
			m_done = new List<IPooledThread>();
			m_maxsimultaneous = simultaneous;
			if (m_maxsimultaneous > MaximumTotalConcurrentThreads)
				m_maxsimultaneous = MaximumTotalConcurrentThreads;
		}


		public void Queue(IPooledThread thread)
		{
			thread.Thread = new Thread(thread.Run);
			m_notstarted.Enqueue(thread);
		}

		public void WaitAll()
		{
			bool any_not_started;
			bool empty_space;
			bool any_running;
			do
			{
				any_not_started = (m_notstarted.Count > 0);
				empty_space = (m_running.Count < m_maxsimultaneous);
				any_running = (m_running.Count > 0);

				// check running if they've finished.
				for (int i = m_running.Count - 1; i >= 0; i--)
				{
					var item = m_running[i];
					if (WaitHandle.WaitAny(new[] { item.Signal }, 1) == 0)
					{
						// Console.WriteLine("Thread in running pool finished.");
						item.Signal.Close(); 
						m_running.RemoveAt(i);
						m_done.Add(item);       										                                                 
					}
				}

				if (any_not_started && empty_space)                                   
				{                                                                                     
					// Console.WriteLine("Starting thread in pool.");
					var startme = m_notstarted.Dequeue();
					startme.Signal = new ManualResetEvent(false);
					// m_events.Add(startme.Signal);
					m_running.Add(startme);
					startme.Thread.Start();
				}

				Thread.Sleep(1);
			}
			while (any_running || any_not_started);
		}
	}
}
