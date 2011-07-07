using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Possan.Distributed.Client
{
	public class ClientConnection
	{
		private readonly ClientConfig _cfg;

		public string JobId { get; set; }
		public ClientConnectionState State { get; set; }

		public ClientConnection(ClientConfig cfg)
		{
			JobId = "";
			_cfg = cfg;
			State = ClientConnectionState.NotStarted;
		}

		public void Start()
		{
			var wc = new WebClient();

			Console.WriteLine("Create job");

			JobId = wc.UploadString(Utilities.CombineURL(_cfg.ManagerUrl, "/createjob"), BuildJobInfoJson());
			Console.WriteLine("Created manager job " + JobId);

			Console.WriteLine("Uploading assemblies...");
			foreach (var a in _cfg.Assemblies)
			{
				if (!File.Exists(a))
					continue;
				wc.UploadData(Utilities.CombineURL(_cfg.ManagerUrl, "/job/" + JobId + "/assemblies"), File.ReadAllBytes(a));
			}
			Console.WriteLine("Uploading done.");

			Console.WriteLine("Starting job "+JobId);
			wc.DownloadString(Utilities.CombineURL(_cfg.ManagerUrl, "/job/" + JobId + "/start"));
			State = ClientConnectionState.Started;
			Console.WriteLine("Job " + JobId+" started."); 
		}

		private string BuildJobInfoJson()
		{
			var jobinfo = new StringBuilder();
			jobinfo.Append("{\"jobtype\":\"").Append(Utilities.EscapeJson(_cfg.JobType)).Append("\",");
			var argjson = Utilities.BuildJsonFromArgs(_cfg.JobArgs);
			if (!string.IsNullOrEmpty(argjson))
				jobinfo.Append("\"jobargs\":" + argjson + ",");
			jobinfo.Append("\"instances\":" + _cfg.Instances + "}");
			return jobinfo.ToString();
		}


		public void Poll()
		{
			if( State != ClientConnectionState.Started )
				return;
			var status = "";
			try
			{
				var wc = new WebClient();
				status = wc.DownloadString(Utilities.CombineURL(_cfg.ManagerUrl, "/job/" + JobId + "/status")).ToLower();
			}
			catch (Exception z)
			{

			}
			if (status == "done")
			{
				Console.WriteLine("Job " + JobId + " is now done.");
				State = ClientConnectionState.Done;
			}
			else if (status != "working")
				Console.WriteLine("Unknown job status: " + status); 
		}

		public void Wait()
		{
			if( State != ClientConnectionState.Started )
				return;

			Console.WriteLine("Waiting for job "+JobId+" to finish...");
			bool done = false;
			do
			{
				Poll();
				Thread.Sleep(1000);
			}
			while (State != ClientConnectionState.Done);
			Console.WriteLine("Job " + JobId + " finished.");
		}

		public bool Run()
		{
			Start();
			Wait();
			return true;
		}
	}

	public enum ClientConnectionState
	{
		NotStarted = 0,
		Started = 1,
		Done = 2
	}
}
