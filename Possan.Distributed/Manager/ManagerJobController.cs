using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using JsonFx.Json;

namespace Possan.Distributed.Manager
{
	public class ManagerJobController
	{
		private readonly Manager _manager;
		private List<ManagerJob> m_jobs;
		private List<WorkerInfo> m_workers;

		public ManagerJobController(Manager manager)
		{
			_manager = manager;
			m_jobs = new List<ManagerJob>();
			m_workers = new List<WorkerInfo>();
		}

		ManagerJob GetJobById(string id)
		{
			return m_jobs.FirstOrDefault(j => j.ID == id);
		}

		WorkerInfo GetWorkerByUrl(string url)
		{
			return m_workers.FirstOrDefault(j => j.URL == url);
		}

		List<string> ProvisionWorkers(int n)
		{
			var ret = new List<string>();
			foreach (var w in m_workers)
			{
				if (w.State != WorkerState.Idle)
					continue;

				w.State = WorkerState.Allocated;
				if (ret.Count < n)
					ret.Add(w.URL);
			}
			return ret;
		}

		public string CreateJob(string jsondata)
		{
			Console.WriteLine("ManagerJobController: Creating job: json=" + jsondata);

			var jr = new JsonReader(jsondata);
			var jx = jr.Deserialize();
			var jo = jx as Dictionary<string, object>;
			if (jo == null)
				return "";

			var newjob = new ManagerJob();

			newjob.ID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

			int numworkers = 2;
			if (jo.ContainsKey("instances"))
				numworkers = int.Parse(jo["instances"].ToString());

			if (jo.ContainsKey("callback"))
				newjob.CallbackUrl = jo["callback"].ToString();

			if (jo.ContainsKey("jobtype"))
				newjob.JobType = jo["jobtype"].ToString();

			if (jo.ContainsKey("jobargs"))
				newjob.JobArgs = new List<string>((string[])jo["jobargs"]);

			Console.WriteLine("ManagerJobController: Trying to get " + numworkers + " instances.");

			var idleworkerlist = ProvisionWorkers(numworkers);
			foreach (var workerurl in idleworkerlist)
			{
				// create job on workers

				Console.WriteLine("ManagerJobController: Telling worker at " + workerurl + " to create a job...");
				var winfo = GetWorkerByUrl(workerurl);
				winfo.RemoteJobId = "";
				var wc = new WebClient();
				var jobinfo = new StringBuilder();
				jobinfo.Append("{\"jobtype\":\"").Append(Utilities.EscapeJson(newjob.JobType)).Append("\",\"jobargs\":[");
				for (int i = 0; i < newjob.JobArgs.Count; i++)
				{
					if (i > 0)
						jobinfo.Append(",");
					var arg = newjob.JobArgs[i];
					jobinfo.Append("\"").Append(Utilities.EscapeJson(arg)).Append("\"");
				}

				var callbackurl = "http://" + _manager.Config.Hostname + ":" + _manager.Config.Port + "/job/" + newjob.ID + "/worker-callback";
				jobinfo.Append("],\"callback\":\"").Append(Utilities.EscapeJson(callbackurl)).Append("\"}");
				try
				{
					winfo.RemoteJobId = wc.UploadString(Utilities.CombineURL(winfo.URL, "/createjob"), jobinfo.ToString());
				}
				catch (Exception z)
				{
					Console.WriteLine(z);
				}
				Console.WriteLine("Created remote job id: " + winfo.RemoteJobId);
				if (winfo.RemoteJobId != "")
					newjob.Workers.Add(winfo);
			}

			Console.WriteLine("ManagerJobController: Got " + newjob.Workers.Count + " workers.");

			//	newjob.CallbackUrl = jo["managerurl"];

			Console.WriteLine("ManagerJobController: Created job " + newjob.ID);//+ " with callback url " + newjob.CallbackUrl);

			m_jobs.Add(newjob);
			return newjob.ID;
		}

		public void AddAssembly(string jobid, byte[] data)
		{
			var job = GetJobById(jobid);
			if (job == null)
				return;

			Console.WriteLine("Uploading one assembly (" + data.Length + " bytes)");
			foreach (var w in job.Workers)
			{
				Console.WriteLine("To " + w.URL);
				var wc = new WebClient();
				wc.UploadData(Utilities.CombineURL(w.URL, "/job/" + w.RemoteJobId + "/assemblies"), data);
			}

			// m_jobs.ass
		}

		public void Start(string jobid)
		{
			var job = GetJobById(jobid);
			if (job == null)
				return;
			job.Start();
		}


		public void JobDone(string jobid, string jsondata)
		{
			var job = GetJobById(jobid);
			if (job == null)
				return;

			var jr = new JsonReader(jsondata);
			var jx = jr.Deserialize();
			var jo = jx as Dictionary<string, object>;
			if (jo == null)
				return;

			var url = jo["url"].ToString();

			Console.WriteLine("ManagerJobController: Worker with url " + url + " is done. (job " + job.ID + ")");

			var worker = GetWorkerByUrl(url);
			if (worker == null)
				return;

			if (worker.State == WorkerState.JobRunning)
				worker.State = WorkerState.JobDone;
		}

		public string GetStatus(string jobid)
		{
			var job = GetJobById(jobid);
			if (job == null)
				return "";

			if (job.State == ManagerJobState.Done)
				return "done";

			return "working";
			// return "";
		}

		public void Keepalive(string jsondata)
		{
			var jr = new JsonReader(jsondata);
			var jx = jr.Deserialize();
			var jo = jx as Dictionary<string, object>;
			if (jo == null)
				return;

			var url = jo["url"].ToString();

			var w = GetWorkerByUrl(url);
			if (w != null)
			{
				w.LastKeepalive = DateTime.Now;
				if (w.State == WorkerState.Dead)
					w.State = WorkerState.Idle;
				return;
			}

			Console.WriteLine("ManagerJobController: Keepalive from new worker: " + url);
			w = new WorkerInfo();
			w.URL = url;
			w.LastKeepalive = DateTime.Now;
			if (w.State == WorkerState.Dead)
				w.State = WorkerState.Idle;
			m_workers.Add(w);
		}

	}
}