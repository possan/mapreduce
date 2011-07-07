using System.Collections.Generic;

namespace Possan.Distributed.Client
{
	public class ClientConfig
	{
		public string ManagerUrl;
		public List<string> Assemblies;
		public string JobType;
		public IJobArgs JobArgs;
		public int Instances;

		public ClientConfig()
		{
			ManagerUrl = "";
			Assemblies = new List<string>();
			JobType = "";
			JobArgs = new DefaultJobArgs();
			Instances = 5;
		}
	}
}