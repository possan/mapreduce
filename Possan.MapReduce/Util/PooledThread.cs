using System.Threading;

namespace Possan.MapReduce.Util
{
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
}