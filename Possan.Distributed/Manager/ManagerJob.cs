using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Possan.Distributed.Manager
{
	public class ManagerJob : SimpleThread
	{
		public string ID;
		public string CallbackUrl;
		public List<WorkerInfo> Workers;
		public ManagerJobState State;
		public ManagerJobController Controller;
		public string JobType;
		public IJobArgs JobArgs;
		public int Timeout;
		public Dictionary<string, byte[]> Assemblies;

		public void AddAssembly(string filename, byte[] data)
		{
			Assemblies.Add(filename, data);
		}

		public ManagerJob()
		{
			ID = ""; // newjob.ID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
			CallbackUrl = "";
			Workers = new List<WorkerInfo>();
			State = ManagerJobState.Preparing;
			JobType = "";
			JobArgs = new DefaultJobArgs();
			Timeout = 5;
			Assemblies = new Dictionary<string, byte[]>();
		}

		public void StartupWorker(WorkerInfo w)
		{
			var wc = new WebClient();
			foreach (var a in Assemblies.Keys)
			{
				Console.WriteLine("To " + w.URL);
				wc.UploadData(Utilities.CombineURL(w.URL, "/job/" + w.RemoteJobId + "/assemblies?name=" + a), Assemblies[a]);
			}
			// upload all assemblies
			var u = Utilities.CombineURL(w.URL, "/job/" + w.RemoteJobId + "/start");
			wc.DownloadString(u);
			w.State = WorkerState.JobRunning;
			w.StartTime = DateTime.Now;
		}

		public override void InnerRun()
		{
			// kör, bara kör!

			Console.WriteLine("Manager job: Tell all workers to start.");

			// upload all assemblies
			foreach (var w in Workers)
			{
				StartupWorker(w);
			}

			State = ManagerJobState.Working;

			Console.WriteLine("Manager job: Wait for all to be done...");

			bool anyNotDoneAndNotCrashed = true;
			while (anyNotDoneAndNotCrashed)
			{
				anyNotDoneAndNotCrashed = false;
				int numcrashed = 0;
				foreach (var w in Workers)
				{
					if (w.State != WorkerState.JobDone && w.State != WorkerState.Crashed)
						anyNotDoneAndNotCrashed = true;
					else 
					{
						var dur = DateTime.Now.Subtract(w.StartTime).TotalSeconds;
						Console.WriteLine("Worker " + w.URL + " is " + w.State + " (runtime: " + dur + " seconds)");
						if (dur > Timeout)
						{
							// mark as crashed?
							w.State = WorkerState.Crashed;
							Console.WriteLine("Crashed? get a new one?");
							numcrashed++;
						}
					}
				}

				if (numcrashed > 0)
				{
					var wurls = Controller.ProvisionWorkers(numcrashed);
					foreach(var wurl in wurls)
					{
						var nw = Controller.GetWorkerByUrl(wurl);
						StartupWorker(nw);
						Workers.Add(nw);
					}
				}

				Thread.Sleep(1000);
			}

			State = ManagerJobState.Done;

			Console.WriteLine("Manager job: Reset worker states to idle.");

			foreach (var w in Workers)
			{
				if( w.State != WorkerState.Crashed )
					w.State = WorkerState.Idle;
			}

			Console.WriteLine("Manager job: Done.");
		}

	}
}