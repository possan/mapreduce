using System.Threading;

namespace Possan.MapReduce
{
	public interface IPooledThread
	{
		Thread Thread { get; set; }
		ManualResetEvent Signal { get; set; }
		void Run();
	}
}