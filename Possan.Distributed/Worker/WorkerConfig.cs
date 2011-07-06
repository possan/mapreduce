using System;

namespace Possan.Distributed.Worker
{
	public class WorkerConfig
	{
		public string ManagerUrl;
		public int MaxThreads;
		public ServerMode Mode;
		public string Hostname;
		public int Port;

		public WorkerConfig()
		{
			ManagerUrl = "";
			Mode = ServerMode.None;
			MaxThreads = 2*Environment.ProcessorCount;
			Hostname = "127.0.0.1";
			Port = 0;
		}

		public enum ServerMode
		{
			None = 0,
			Worker = 1,
			Manager = 2
		}
	}
}