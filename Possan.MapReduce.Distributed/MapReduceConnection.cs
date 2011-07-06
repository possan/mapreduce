using System;
using System.Collections.Generic;
using Possan.Distributed.Client;

namespace Possan.MapReduce.Distributed
{
	public class MapReduceConnection
	{
		public string ManagerUrl;

		public string InputFolder;
		public string OutputFolder;
		public string TempFolder;

		public string InputPartitionerTypeName;
		public string MapperTypeName;
		public string ReducerTypeName;
		public string PreReducerTypeName;
		public string ShufflerPartitioner;

		public int Instances;
		public int NumInputPartitions;

		public List<string> Assemblies;
		public string OutputType;
		public string InputType;

		public MapReduceConnection()
		{
			ManagerUrl = "";

			InputFolder = "";
			InputType = "";
			TempFolder = "";
			OutputFolder = "";
			OutputType = "";

			Instances = 5;
			NumInputPartitions = 10;

			InputPartitionerTypeName = "";
			MapperTypeName = "";
			ReducerTypeName = "";
			PreReducerTypeName = "";
			ShufflerPartitioner = "";

			Assemblies = new List<string>();
			Assemblies.Add("Possan.Distributed.dll");
			Assemblies.Add("Possan.MapReduce.Distributed.dll");
		}

		public void Run()
		{
			var TempRW = "Possan.MapReduce.IO.TabFileFolder, Possan.MapReduce";
			
			// partition input... N outputs

			Console.WriteLine("==============================================");

			{
				var cfg = new ClientConfig();
				cfg.Assemblies = Assemblies;
				cfg.ManagerUrl = ManagerUrl;
				cfg.Instances = 1;
				cfg.JobType = "Possan.MapReduce.Distributed.PartitionInputJob, Possan.MapReduce.Distributed";
				cfg.JobArgs.Add(InputPartitionerTypeName);
				cfg.JobArgs.Add(InputType);
				cfg.JobArgs.Add(TempRW);
				cfg.JobArgs.Add(InputFolder);
				for (int k = 0; k < NumInputPartitions; k++)
					cfg.JobArgs.Add(TempFolder + "\\partitioned-input-" + k);
				var conn = new ClientConnection(cfg);
				conn.Run();
			}

			Console.WriteLine("==============================================");


			// run N mappers+prereducers + join to N reducers
			{
				var allmappers = new List<ClientConnection>();
				for (int k = 0; k < NumInputPartitions; k++)
				{

					var cfg = new ClientConfig();
					cfg.Assemblies = Assemblies;
					cfg.ManagerUrl = ManagerUrl;
					cfg.Instances = Instances;
					cfg.JobType = "Possan.MapReduce.Distributed.MapJob, Possan.MapReduce.Distributed";
					cfg.JobArgs.Add(MapperTypeName);
					cfg.JobArgs.Add(TempRW);
					cfg.JobArgs.Add(TempRW);
					cfg.JobArgs.Add(TempFolder + "\\partitioned-input-" + k);
					cfg.JobArgs.Add(TempFolder + "\\mapper-output-" + k);

					var conn = new ClientConnection(cfg);
					conn.Start();
					allmappers.Add(conn);
				}

				Console.WriteLine("==============================================");
				
				for (int k = 0; k < NumInputPartitions; k++)
				{
					allmappers[k].Wait();
				}
			}

			Console.WriteLine("==============================================");


			/*
			// run N reducers 
			{
				var cfg = new ClientConfig();
				cfg.Assemblies = Assemblies;
				cfg.ManagerUrl = ManagerUrl;
				cfg.Instances = 1;
				cfg.JobType = "Possan.MapReduce.Distributed.MapJob, Possan.MapReduce.Distributed";
				cfg.JobArgs.Add("a");
				cfg.JobArgs.Add("b");
				cfg.JobArgs.Add("c");

				var conn = new ClientConnection(cfg);
				conn.Run();
			}

			// join data 

			*/

			/*
			{
				var cfg = new ClientConfig();
				cfg.Assemblies = Assemblies;
				cfg.ManagerUrl = ManagerUrl;
				cfg.Instances = Instances;
				cfg.JobType = "Possan.MapReduce.Distributed.MapJob, Possan.MapReduce.Distributed";
				cfg.JobArgs.Add("a");
				cfg.JobArgs.Add("b");
				cfg.JobArgs.Add("c");

				var conn = new ClientConnection(cfg);
				conn.Run();
			}*/
		}
	}
}
