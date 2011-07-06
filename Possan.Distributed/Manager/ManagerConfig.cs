namespace Possan.Distributed.Manager
{
	public class ManagerConfig
	{
		public string Hostname;
		public int Port;

		public ManagerConfig()
		{
			Hostname = "127.0.0.1";
			Port = 10001;
		}
	}
}