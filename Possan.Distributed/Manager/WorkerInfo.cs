using System;

namespace Possan.Distributed.Manager
{
	public class WorkerInfo
	{
		public string URL;
		public DateTime LastKeepalive;
		public string RemoteJobId;
		public WorkerState State;
		public DateTime StartTime;
		public bool IsAlive { get { return DateTime.Now.Subtract(LastKeepalive).TotalSeconds < 20; } }

		public WorkerInfo()
		{
			State = WorkerState.Dead;
			URL = "";
			RemoteJobId = "";
			StartTime = DateTime.Now;
			LastKeepalive = DateTime.MinValue;
		}
	}
}