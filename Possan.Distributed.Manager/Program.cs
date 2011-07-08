using System;
using NDesk.Options;

namespace Possan.Distributed.Manager.Application
{
	class Program
	{ 
		static void Main(string[] args)
		{
			string host = "127.0.0.1";
			bool help = false;
			int port = 8000;
			var p = new OptionSet {
   				{ "h|host=", v => host = v },
   				{ "p|port=", v => port = int.Parse(v) },
   				{ "?|help", v => help = v != null },
			};
			p.Parse(args);

			if (help)
			{
				p.WriteOptionDescriptions(Console.Out);
				// Syntax();
				return;
			}

			// set up manager
			var mgr = new Manager(new ManagerConfig { Port = port, Hostname = host });
			mgr.Start();

			Console.WriteLine("Worker(s) running, hit [enter] to exit");
			Console.ReadLine();

			mgr.Stop();
		}

	}
}
