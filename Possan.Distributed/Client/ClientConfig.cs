using System.Collections.Generic;

namespace Possan.Distributed.Client
{
	public class ClientConfig
	{
		public string ManagerUrl;
		public List<string> Assemblies;
		public string JobType;
		public List<string> JobArgs;
		public int Instances;

		public ClientConfig()
		{
			ManagerUrl = "";
			Assemblies = new List<string>();
			JobType = "";
			JobArgs = new List<string>();
			Instances = 5;
		}
	}
}