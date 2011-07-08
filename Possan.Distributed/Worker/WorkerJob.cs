using System;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Policy;
using Possan.Distributed.Sandbox;

namespace Possan.Distributed.Worker
{
	class WorkerJob : SimpleThread
	{
		public string ID;
		public string MyUrl;
		public string CallbackUrl;
		// public List<Assembly> LoadedAssemblies;
		private AppDomain Sandbox;
		public string SandboxPath;
		public string JobType;
		public IJobArgs JobArgs;

		public WorkerJob()
		{
			// LoadedAssemblies = new List<Assembly>();
			SandboxPath = Path.GetTempPath() + Path.DirectorySeparatorChar + "wa-" + Guid.NewGuid().ToString();
			if (!Directory.Exists(SandboxPath))
				Directory.CreateDirectory(SandboxPath);
			var setup = new AppDomainSetup();
			setup.ApplicationBase = SandboxPath;
			var perms = new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
			Sandbox = AppDomain.CreateDomain("Temp sandbox", null, setup, perms, new StrongName[] { });
			JobType = "";
			JobArgs = new DefaultJobArgs();
		}

		public override void InnerRun()
		{
			// kör, bara kör!
			Console.WriteLine("WorkerJob: Started, JobType: " + JobType);

			try
			{
				var sandboxargs = new SandboxedJobArgs();
				foreach (var k in JobArgs.GetKeys())
					foreach (var v in JobArgs.GetValues(k))
						sandboxargs.Add(k, v);

				var sandboxtool = Sandbox.CreateInstanceAndUnwrap("Possan.Distributed.Sandbox", "Possan.Distributed.Sandbox.SandboxProxy") as ISandboxProxy;
				if (sandboxtool != null)
					sandboxtool.RunJob(Sandbox, JobType, sandboxargs);
			}
			catch (Exception z)
			{
				Console.WriteLine("Creating mapper failed: " + z);
			}

			var wc = new WebClient();
			var postdata = "{\"url\":\"" + MyUrl + "\",\"job\":\"" + ID + "\"}";
			wc.UploadString(CallbackUrl, postdata);

			Console.WriteLine("WorkerJob: All done, clean up.");

			// delete folder...
		}

		public void AddAssembly(string filename, byte[] data)
		{
			try
			{
				Console.WriteLine("Loading " + filename + " (" + data.Length + " bytes)");
				File.WriteAllBytes(SandboxPath + Path.DirectorySeparatorChar + filename, data);
				// var a = Sandbox.Load(filename);
				// Console.WriteLine("Loaded: " + a.FullName);
			}
			catch (Exception z)
			{
				Console.WriteLine("Failed to load assembly: " + z);
			}
		}
	}
}