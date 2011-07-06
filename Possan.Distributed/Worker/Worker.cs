using System;
using System.Net;
using System.Threading;

namespace Possan.Distributed.Worker
{
	public class Worker
	{
		public WorkerConfig Config;
		public WorkerJobController JobController;
		private HttpServer.HttpServer _server;
		private KeepaliveThread _keepalive;

		public Worker(WorkerConfig config)
		{
			Config = config;
			JobController = new WorkerJobController(this);
		}

		public void Start()
		{
			_server = new HttpServer.HttpServer();
			_server.Add(new WorkerRequestHandler(this));
			_server.Start(Dns.GetHostEntry(Config.Hostname).AddressList[0], Config.Port);

			_keepalive = new KeepaliveThread(this);
			_keepalive.Start();
			
			Thread.Sleep(10);
		}

		public void Stop()
		{
			_keepalive.Stop();
			_server.Stop();
		}

		public void Wait()
		{
			Console.WriteLine("Running worker; connecting to manager at " + Config.ManagerUrl);
			Console.ReadLine();
		}
	}
}