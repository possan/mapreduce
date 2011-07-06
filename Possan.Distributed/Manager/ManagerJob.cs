using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Possan.Distributed.Manager
{
	class ManagerJob : SimpleThread
	{
		public string ID;
		public string CallbackUrl;
		public List<WorkerInfo> Workers;
		public ManagerJobState State;
		public string JobType;
		public List<string> JobArgs;

		public ManagerJob()
		{
			ID = ""; // newjob.ID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
			CallbackUrl = "";
			Workers = new List<WorkerInfo>();
			State = ManagerJobState.Preparing;
			JobType = "";
			JobArgs = new List<string>();
		}

		public override void InnerRun()
		{
			// kör, bara kör!

			Console.WriteLine("Manager job: Tell all workers to start.");

			foreach (var w in Workers)
			{
				var wc = new WebClient();
				var u = Utilities.CombineURL(w.URL, "/job/" + w.RemoteJobId + "/start");
				wc.DownloadString(u);
				w.State = WorkerState.JobRunning;
			}

			State = ManagerJobState.Working;

			Console.WriteLine("Manager job: Wait for all to be done...");

			bool _any_not_done = true;
			while (_any_not_done)
			{
				_any_not_done = false;
				foreach (var w in Workers)
				{
					if (w.State != WorkerState.JobDone)
						_any_not_done = true;
					else
						Console.WriteLine("Worker " + w.URL + " is " + w.State);
				}
				Thread.Sleep(2500);
			}

			State = ManagerJobState.Done;

			Console.WriteLine("Manager job: Reset worker states to idle.");

			foreach (var w in Workers)
			{
				w.State = WorkerState.Idle;
			}

			Console.WriteLine("Manager job: Done.");
		}

	}
}