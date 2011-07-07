using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

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
		public IJobArgs JobArgs;
		
		public WorkerJob()
		{
			LoadedAssemblies = new List<Assembly>();
			Sandbox = AppDomain.CreateDomain("Temp sandbox");
			JobType = "";
			JobArgs = new DefaultJobArgs();
		}

		public override void InnerRun()
		{
			// kör, bara kör!
			Console.WriteLine("WorkerJob: Started, JobType: " + JobType);

			try
			{
				var sandboxtool = Sandbox.CreateInstanceAndUnwrap("Possan.Distributed", "Possan.Distributed.SandboxProxy") as ISandboxProxy;
				if (sandboxtool != null)
					sandboxtool.RunJob(JobType, JobArgs);
			}
			catch (Exception z)
			{
				Console.WriteLine("Creating mapper failed: " + z);
			}

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