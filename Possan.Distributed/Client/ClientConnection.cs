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

		public ClientConnection(ClientConfig cfg)
		{
			JobId = "";
			_cfg = cfg;
		}
		
		public bool Start()
		{
			var wc = new WebClient();

			Console.WriteLine("Create job");

			var jobinfo = new StringBuilder();
			jobinfo.Append("{\"jobtype\":\"").Append(Utilities.EscapeJson(_cfg.JobType)).Append("\",\"jobargs\":[");
			for (int i = 0; i < _cfg.JobArgs.Count; i++)
			{
				if (i > 0)
					jobinfo.Append(",");
				var arg = _cfg.JobArgs[i];
				jobinfo.Append("\"").Append(Utilities.EscapeJson(arg)).Append("\"");
			}

			jobinfo.Append("],\"instances\":" + _cfg.Instances + "}");

			JobId = wc.UploadString(Utilities.CombineURL(_cfg.ManagerUrl, "/createjob"), jobinfo.ToString());
			Console.WriteLine("Created manager job " + JobId);

			Console.WriteLine("Uploading assemblies...");
			foreach (var a in _cfg.Assemblies)
			{
				if (!File.Exists(a))
					continue;
				wc.UploadData(Utilities.CombineURL(_cfg.ManagerUrl, "/job/" + JobId + "/assemblies"), File.ReadAllBytes(a));
			}
			Console.WriteLine("Uploading done.");
			 
			Console.WriteLine("Start job.");
			wc.DownloadString(Utilities.CombineURL(_cfg.ManagerUrl, "/job/" + JobId + "/start"));

			return true;
		}


		public bool Wait()
		{ 
			var wc = new WebClient();
			 
			bool done = false;
			do
			{
				var status = wc.DownloadString(Utilities.CombineURL(_cfg.ManagerUrl, "/job/" + JobId + "/status")).ToLower();
				if (status == "done")
					done = true;
				else if (status != "working")
					Console.WriteLine("Unknown job status: " + status);
				Thread.Sleep(1000);
			}
			while (!done);

			return true;
		}

		public bool Run()
		{ 
			Start();
			Wait();
			return true;
		}
	}
}
