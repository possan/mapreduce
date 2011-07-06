using System.Net;
using System.Threading;

namespace Possan.Distributed.Worker
{
	public class KeepaliveThread : SimpleThread
	{
		private readonly Worker _worker;

		public KeepaliveThread(Worker worker)
		{
			_worker = worker;
		}

		void SendKeepaliveToManager()
		{
			var wc = new WebClient();
			var reginfo = "{\"url\":\"http://" + _worker.Config.Hostname + ":" + _worker.Config.Port + "\",\"utilization\":0}";
			wc.UploadString(Utilities.CombineURL(_worker.Config.ManagerUrl, "/keepalive"), reginfo);
		}

		public override void InnerRun()
		{
			while (KeepRunning)
			{
				SendKeepaliveToManager();
				Thread.Sleep(10000);
			}
		}
	}
}