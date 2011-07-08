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
		public string ShufflerPartitionerTypeName;
		public string CombinePartitionerTypeName;

		public int Instances;
		public int NumInputPartitions;
		public int NumReducerPartitions;

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

			Instances = 1;
			NumInputPartitions = 4;
			NumReducerPartitions = 7;

			InputPartitionerTypeName = "";
			MapperTypeName = "";
			ReducerTypeName = "";
			PreReducerTypeName = "";
			ShufflerPartitionerTypeName = "";

			InputPartitionerTypeName = "Possan.MapReduce.Partitioners.MD5Partitioner, Possan.MapReduce";
			ShufflerPartitionerTypeName = "Possan.MapReduce.Partitioners.MD5Partitioner, Possan.MapReduce";
			CombinePartitionerTypeName = "Possan.MapReduce.Partitioners.MD5Partitioner, Possan.MapReduce";

			Assemblies = new List<string>();
			// Assemblies.Add("HttpServer.dll");
			// Assemblies.Add("JsonFx.Json.dll");
			Assemblies.Add("Possan.MapReduce.dll");
			Assemblies.Add("Possan.Distributed.Sandbox.dll");
			Assemblies.Add("Possan.MapReduce.Distributed.Jobs.dll");
		}

		public void Run()
		{
			var TempRW = "Possan.MapReduce.IO.TabFileFolder, Possan.MapReduce";

			// partition input... N outputs

			Console.WriteLine("==============================================");

			var partitioners = new ClientConnectionCollection();
			var partitionconfig = new ClientConfig();
			partitionconfig.Assemblies = Assemblies;
			partitionconfig.ManagerUrl = ManagerUrl;
			partitionconfig.Instances = 1;
			partitionconfig.JobType = "Possan.MapReduce.Distributed.Jobs.ShuffleJob, Possan.MapReduce.Distributed.Jobs";
			partitionconfig.JobArgs.Add("partitioner", InputPartitionerTypeName);
			partitionconfig.JobArgs.Add("sort", "1");
			partitionconfig.JobArgs.Add("input", InputType + "=" + InputFolder);
			for (var k = 0; k < NumInputPartitions; k++)
				partitionconfig.JobArgs.Add("output", TempRW + "=" + TempFolder + "\\partitioned-input-" + k);
			partitioners.Add(partitionconfig);
			partitioners.StartAllAndWait();

			Console.WriteLine("==============================================");

			// run N mappers+prereducers + join to N reducers
			var mappers = new ClientConnectionCollection();
			for (int k = 0; k < NumInputPartitions; k++)
			{
				var mapperconfig = new ClientConfig();
				mapperconfig.Assemblies = Assemblies;
				mapperconfig.ManagerUrl = ManagerUrl;
				mapperconfig.Instances = 1;
				mapperconfig.JobType = "Possan.MapReduce.Distributed.Jobs.MapJob, Possan.MapReduce.Distributed.Jobs";
				mapperconfig.JobArgs.Add("mapper", MapperTypeName);
				mapperconfig.JobArgs.Add("input", TempRW + "=" + TempFolder + "\\partitioned-input-" + k);
				mapperconfig.JobArgs.Add("output", TempRW + "=" + TempFolder + "\\mapper-output-" + k);
				mappers.Add(mapperconfig);
			}
			mappers.StartAllAndWait();

			Console.WriteLine("==============================================");

			// shuffle/sort to N reducers

			var sorters = new ClientConnectionCollection();
			for (int k = 0; k < NumInputPartitions; k++)
			{
				var sorterconfig = new ClientConfig();
				sorterconfig.Assemblies = Assemblies;
				sorterconfig.ManagerUrl = ManagerUrl;
				sorterconfig.Instances = 1;
				sorterconfig.JobType = "Possan.MapReduce.Distributed.Jobs.ShuffleJob, Possan.MapReduce.Distributed.Jobs";
				sorterconfig.JobArgs.Add("partitioner", ShufflerPartitionerTypeName);
				sorterconfig.JobArgs.Add("sort", "1");
				sorterconfig.JobArgs.Add("input", TempRW + "=" + TempFolder + "\\mapper-output-" + k);
				for (var u = 0; u < NumReducerPartitions; u++)
					sorterconfig.JobArgs.Add("output", TempRW + "=" + TempFolder + "\\reducer-input-" + u);
				sorters.Add(sorterconfig);
			}
			sorters.StartAllAndWait();

			Console.WriteLine("==============================================");

			// run N reducers 

			var reducers = new ClientConnectionCollection();
			for (int k = 0; k < NumReducerPartitions; k++)
			{
				var reducerconfig = new ClientConfig();
				reducerconfig.Assemblies = Assemblies;
				reducerconfig.ManagerUrl = ManagerUrl;
				reducerconfig.Instances = 1;
				reducerconfig.JobType = "Possan.MapReduce.Distributed.Jobs.ReduceJob, Possan.MapReduce.Distributed.Jobs";
				reducerconfig.JobArgs.Add("reducer", ReducerTypeName);
				reducerconfig.JobArgs.Add("input", TempRW + "=" + TempFolder + "\\reducer-input-" + k);
				reducerconfig.JobArgs.Add("output", TempRW + "=" + TempFolder + "\\reducer-output-" + k);
				reducers.Add(reducerconfig);
			}
			reducers.StartAllAndWait();
			
			Console.WriteLine("==============================================");
			
			// combine

			var combiners = new ClientConnectionCollection();

			var combinerconfig = new ClientConfig();
			combinerconfig.Assemblies = Assemblies;
			combinerconfig.ManagerUrl = ManagerUrl;
			combinerconfig.Instances = 1;
			combinerconfig.JobType = "Possan.MapReduce.Distributed.Jobs.ShuffleJob, Possan.MapReduce.Distributed.Jobs";
			combinerconfig.JobArgs.Add("partitioner", CombinePartitionerTypeName );
			combinerconfig.JobArgs.Add("sort", "1");
			for (var u = 0; u < NumReducerPartitions; u++)
				combinerconfig.JobArgs.Add("input", TempRW + "=" + TempFolder + "\\reducer-output-" + u);
			combinerconfig.JobArgs.Add("output", OutputType + "=" + OutputFolder );
			combiners.Add(combinerconfig);
			combiners.StartAllAndWait();

			Console.WriteLine("==============================================");
		}
	}
}
