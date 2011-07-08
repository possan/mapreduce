using System;
using System.Collections.Generic;
using NDesk.Options;

namespace Possan.Distributed.Worker
{
	class Program
	{
		static void Main(string[] args)
		{
			string host = "127.0.0.1";
			string manager = "";
			bool help = false;
			int port = 8000;
			int threads = Environment.ProcessorCount;
			int instances = 1;
			var p = new OptionSet {
   				{ "m|manager=", v => manager = v },
   				{ "h|host=", v => host = v },
   				{ "p|port=", v => port = int.Parse(v) },
   				{ "i|instances=", v => instances = int.Parse(v) },
   				{ "t|threads=", v => threads = int.Parse(v) },
   				{ "?|help", v => help = v != null },
			};
			p.Parse(args);
			if (help || string.IsNullOrEmpty(manager ))
			{
				p.WriteOptionDescriptions(Console.Out);
				// Syntax();
				return;
			}

			// set up workers
			var workers = new List<Worker>();
			for (int i = 0; i < instances; i++)
			{
				var w = new Worker(new WorkerConfig { ManagerUrl = manager, Port = port + i, MaxThreads = threads });
				workers.Add(w);
				w.Start();
			}

			Console.WriteLine("Worker(s) running, hit [enter] to exit");
			Console.ReadLine();

			// kill workers );
			foreach (var w in workers)
				w.Stop();
		}
	}
}
