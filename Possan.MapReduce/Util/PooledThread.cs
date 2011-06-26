using System.Threading;
using System;

namespace Possan.MapReduce.Util
{
	public class PooledThread : IPooledThread
	{
		public Thread Thread { get; set; }
		public ManualResetEvent Signal { get; set; }

		public void Run()
		{
			try{
				InnerRun();
				// Console.WriteLine( "Thread done, delay then signal." );
			}catch(Exception e){	
				Console.WriteLine( "Thread failed: "+e );
			}
			Thread.Sleep(10);
			Signal.Set();
		}

		virtual public void InnerRun()
		{
		}
	}
}