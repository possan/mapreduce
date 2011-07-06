using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;

namespace Possan.Distributed.Worker
{
	class WorkerJob : SimpleThread
	{
		public string ID;
		public string MyUrl;
		public string CallbackUrl;

		public List<Assembly> LoadedAssemblies;
		private AppDomain Sandbox;
		public string JobType;
		public List<string> JobArgs;

		static Random r = new Random();

		public WorkerJob()
		{
			LoadedAssemblies = new List<Assembly>();
			Sandbox = AppDomain.CreateDomain("Temp sandbox");
			JobType = "";
			JobArgs = new List<string>();

		}

		public override void InnerRun()
		{
			// kör, bara kör!
			Console.WriteLine("WorkerJob: Started, JobType: " + JobType);
			foreach (var a in JobArgs)
				Console.WriteLine("WorkerJob: JobArgs[]: " + a);

			try
			{
				var sandboxtool = Sandbox.CreateInstanceAndUnwrap("Possan.Distributed", "Possan.Distributed.SandboxProxy") as ISandboxProxy;
				// Console.WriteLine("created sandboxtool: " + sandboxtool);
				var ret = sandboxtool.RunJob(JobType, JobArgs.ToArray());
			}
			catch (Exception z)
			{
				Console.WriteLine("Creating mapper failed: " + z);
			}

			// var d = r.Next(2000, 5000);
			// Console.WriteLine("WorkerJob: Sleep for " + d + " ms.");
			// Thread.Sleep(d);

			// Console.WriteLine("WorkerJob: Worker, tell manager...");
			var wc = new WebClient();
			var postdata = "{\"url\":\"" + MyUrl + "\",\"job\":\"" + ID + "\"}";
			wc.UploadString(CallbackUrl, postdata);

			Console.WriteLine("WorkerJob: All done, clean up.");
		}

		public void AddAssembly(byte[] data)
		{
			try
			{
				var a = Sandbox.Load(data);
				if (a != null)
					LoadedAssemblies.Add(a);
			}
			catch (Exception z)
			{
				Console.WriteLine("Failed to load assembly: " + z);
			}
		}
	}
}