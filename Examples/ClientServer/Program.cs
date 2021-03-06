﻿using System;
using System.Threading;
using Possan.MapReduce.Distributed;
using Possan.MapReduce.Util;

namespace ClientServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var timer = new Timing("All");

			Thread.Sleep(4000);

			/*
			int baseport = 7890;
			// set up manager
			var mgr = new Manager(new ManagerConfig { Port = baseport });
			mgr.Start();
			// set up workers
			var workers = new List<Worker>();
			for (int i = 0; i < 20; i++)
			{
				var w = new Worker(new WorkerConfig { ManagerUrl = "http://127.0.0.1:" + baseport, Port = baseport + 1 + i, MaxThreads = 1 });
				workers.Add(w);
				w.Start();
			}
			*/

			/*
			var ccfg = new ClientConfig();
			ccfg.Assemblies.Add("Possan.MapReduce.dll");
			ccfg.Assemblies.Add("Possan.Distributed.Sandbox.dll");
			ccfg.Assemblies.Add("ClientServer.Shared.dll");
			ccfg.JobType = "ClientServer.Shared.TestJob, ClientServer.Shared";
			ccfg.JobArgs.Add("test","hej");
			ccfg.ManagerUrl = "http://127.0.0.1:8000";
			ccfg.Instances = 1;
			var client = new ClientConnection(ccfg);
			client.Run();
			
			ccfg = new ClientConfig();
			ccfg.Assemblies.Add("Possan.Distributed.dll");
			ccfg.Assemblies.Add("Possan.MapReduce.Distributed.dll");
			ccfg.Assemblies.Add("ClientServer.Shared.dll");
			ccfg.JobType = "ClientServer.Shared.TestJob, ClientServer.Shared";
			ccfg.JobArgs = new List<string> { "a2", "a3", "a4" };
			ccfg.ManagerUrl = "http://127.0.0.1:" + baseport;
			ccfg.Instances = 4;
			client = new ClientConnection(ccfg);
			client.Run();
			*/

			var suffix = DateTime.Now.Ticks.ToString();// Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
			var conn = new MapReduceConnection();
			conn.ManagerUrl = "http://127.0.0.1:8000";// +baseport;
			conn.InputFolder = "c:\\temp\\smalltextfiles";
			conn.InputType = "Possan.MapReduce.IO.TextFilesFolderSource, Possan.MapReduce";
			conn.TempFolder = "c:\\temp\\MR4\\temp-" + suffix;
			conn.OutputFolder = "c:\\temp\\MR4\\output-" + suffix;
			conn.OutputType = "Possan.MapReduce.IO.TabFileFolder, Possan.MapReduce";
			conn.Assemblies.Add("ClientServer.Shared.dll");
			conn.Instances = 8;
			conn.NumInputPartitions = 5;
			conn.NumReducerPartitions = 4;
			conn.InputPartitionerTypeName = "Possan.MapReduce.Partitioners.MD5Partitioner, Possan.MapReduce";
			conn.ShufflerPartitionerTypeName = "Possan.MapReduce.Partitioners.FirstCharacterPartitioner, Possan.MapReduce";
			conn.CombinePartitionerTypeName = "Possan.MapReduce.Partitioners.FirstCharacterPartitioner, Possan.MapReduce";
			conn.MapperTypeName = "ClientServer.Shared.TestMapper, ClientServer.Shared";
			conn.ReducerTypeName = "ClientServer.Shared.TestReducer, ClientServer.Shared";
			conn.Run();

			/*
			// kill workers 
			foreach (var w in workers)
				w.Stop();
			// stop manager
			mgr.Stop();
			*/
			timer.End();

			Console.WriteLine("Hit [enter] to exit");
			Console.ReadLine();

		}
	}
}
