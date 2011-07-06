using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

namespace Possan.Distributed.Worker
{
	public class WorkerJobController
	{
		private readonly Worker _worker;
		private List<WorkerJob> m_jobs;

		public WorkerJobController(Worker worker)
		{
			_worker = worker;
			m_jobs = new List<WorkerJob>();
		}

		WorkerJob GetJobById(string id)
		{
			return m_jobs.FirstOrDefault(j => j.ID == id);
		}

		public string CreateJob(string jsondata)
		{
			var newjob = new WorkerJob();

			newjob.ID = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

			var jr = new JsonReader(jsondata);
			var jo = jr.Deserialize() as Dictionary<string, object>;

			if (jo.ContainsKey("callback"))
				newjob.CallbackUrl = jo["callback"].ToString();

			if (jo.ContainsKey("jobtype"))
				newjob.JobType = jo["jobtype"].ToString();

			if (jo.ContainsKey("jobargs"))
				newjob.JobArgs = new List<string>((string[])jo["jobargs"]);

			newjob.MyUrl = "http://" + _worker.Config.Hostname + ":" + _worker.Config.Port;

			Console.WriteLine("WorkerJobController: Created job #" + newjob.ID + " with callback url " + newjob.CallbackUrl);

			m_jobs.Add(newjob);
			return newjob.ID;
		}

		public void AddAssembly(string jobid, byte[] data)
		{
			var job = GetJobById(jobid);
			if (job == null)
				return;
			job.AddAssembly(data);
			// m_jobs.ass
		}

		public void Start(string jobid)
		{
			var job = GetJobById(jobid);
			if (job == null)
				return;
			job.Start();
		}
	}
}