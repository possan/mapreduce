using System;

namespace Possan.Distributed.Sandbox
{
	public class SandboxProxy : MarshalByRefObject, ISandboxProxy
	{
		public string RunJob(AppDomain domain, string jobtype, ISandboxedJobArgs args)
		{
			string ret = "";
			Console.WriteLine("SandboxProxy: Trying to create a " + jobtype);
			try
			{
				var aa = jobtype.Split(',');

				var t = domain.CreateInstanceAndUnwrap(aa[1].Trim(), aa[0].Trim()) as ISandboxedJob;
				// var t = Activator.CreateInstance(Type.GetType(jobtype)) as ISandboxedJob;
				Console.WriteLine("Created type " + t);
				Console.WriteLine("---------------------------------------------------------");
				ret = t.Run(args);
				Console.WriteLine("---------------------------------------------------------");
			}
			catch (Exception z)
			{
				Console.WriteLine(z);
			}
			return ret;
		}
	}
}
