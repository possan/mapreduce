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

		public static int MaximumTotalConcurrentThreads = 10;

		private int m_maxsimultaneous;
		private string m_name;

		public ThreadPool(int simultaneous, string name)
			: this(simultaneous)
		{
			m_name = name;
		}

		public ThreadPool(int simultaneous)
		{
			m_name = "";
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

		public void Step()
		{
			if( m_running.Count > 0 )
				for (int i = m_running.Count - 1; i >= 0; i--)
				{
					var item = m_running[i];
					// Console.WriteLine("Checking for signal: "+item.Signal+" (#"+i+")");
					var test = false;
					try{
						test = item.Signal.WaitOne(0,true);
					} catch( Exception z ){
					}
					if( test )
					{
						item.Signal.Close();
						m_running.RemoveAt(i);
						m_done.Add(item);
					//	Console.WriteLine("Got signal for #"
					//		+ i + ", "
					//		+ m_done.Count + " items done, "
					//		+ m_running.Count + " running.");
					}
				}
			bool any_not_started = (m_notstarted.Count > 0);
			bool empty_space = (m_running.Count < m_maxsimultaneous);
			if (any_not_started && empty_space)
			{
				var startme = m_notstarted.Dequeue();
				startme.Signal = new ManualResetEvent(false);
				m_running.Add(startme);
				startme.Thread.Start();
				// Console.WriteLine("Started another one. "+m_running.Count+" running, "+m_notstarted.Count+" queued.");
			}
		}

		public void WaitAll()
		{
			bool any_not_started;
			bool any_running;
			var nextdump = DateTime.Now.AddSeconds(1);
			do
			{
				Step();
				if (DateTime.Now > nextdump)
				{
					Console.WriteLine("Threadpool \"" 
						+ m_name + "\" waiting... [" 
						+ m_running.Count + " running, " 
						+ m_notstarted.Count + " in queue, " 
						+ m_done.Count + " done.]");					
					nextdump = DateTime.Now.AddSeconds(2);
				}
				Thread.Sleep(1);
				any_running = (m_running.Count > 0);
				any_not_started = (m_notstarted.Count > 0);
			}
			while (any_running || any_not_started);
			Console.WriteLine( "Threadpool WaitAll Done.");
		}
	}
}