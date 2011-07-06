using System;
using System.Threading;

namespace Possan.Distributed
{
	public class SimpleThread
	{
		private Thread m_thread;
		private ManualResetEvent m_killsignal;

		public void Start()
		{
			m_killsignal = new ManualResetEvent(false);
			m_thread = new Thread(Run);
			m_thread.Start();
		}

		public void Stop()
		{
			m_killsignal.Set();
			Thread.Sleep(100);
			m_thread.Abort();
		}

		protected bool KeepRunning
		{
			get
			{
				return !m_killsignal.WaitOne(1);
			}
		}

		public virtual void InnerRun()
		{
			// override me
		}

		public void Run()
		{
			try
			{
				InnerRun();
			}
			catch (Exception)
			{
			}
			// if( m_killsignal != null )
			m_killsignal.Close();
		}
	}
}
