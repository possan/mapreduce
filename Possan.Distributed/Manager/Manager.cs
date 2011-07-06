using System;
using System.Net;
using System.Threading;

namespace Possan.Distributed.Manager
{
	public class Manager
	{
		public ManagerConfig Config;
		private HttpServer.HttpServer _server;
		public ManagerJobController Controller;
		
		public Manager(ManagerConfig config)
		{
			Config = config;
			Controller = new ManagerJobController(this);
		}

		public void Start()
		{
			Console.WriteLine("Manager: start listening for connections on " + Config.Hostname + " port " + Config.Port);
			
			_server = new HttpServer.HttpServer();
			_server.Add(new ManagerRequestHandler(this));
			_server.Start(Dns.GetHostEntry(Config.Hostname).AddressList[0], Config.Port);

			Thread.Sleep(10);
		}

		public void Stop()
		{
			_server.Stop();
		}

		public void Wait()
		{
			Console.WriteLine("Manager: wait, hit any key to continue");
			Console.ReadLine();
		}
	}
}