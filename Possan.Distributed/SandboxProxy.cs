using System;

namespace Possan.Distributed
{
	public class SandboxProxy : MarshalByRefObject, ISandboxProxy
	{
		public string RunJob(string jobtype, string[] args)
		{
			string ret = "";
			Console.WriteLine("SandboxProxy: Trying to create a " + jobtype);
			try
			{
				var t = Activator.CreateInstance(Type.GetType(jobtype)) as ISandboxedJob;
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
