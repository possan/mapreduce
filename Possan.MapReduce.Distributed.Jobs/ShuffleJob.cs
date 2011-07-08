using System;
using System.Threading;
using Possan.Distributed.Sandbox;
using Possan.MapReduce.Util;

namespace Possan.MapReduce.Distributed.Jobs
{
	public class ShuffleJob : ISandboxedJob
	{
		public string Run(ISandboxedJobArgs args)
		{
			Console.WriteLine("Inside shuffle job");

			string partitionertype = args.Get("partitioner");
			var partitioner = Activator.CreateInstance(Type.GetType(partitionertype)) as IShardingPartitioner;
			
			bool sort = (int.Parse(args.Get("sort", "1")) == 1);
			
			var inputs = JobUtilities.ParseAndCreateFileSources(args.GetValues("input"));
			
			var outputs = JobUtilities.ParseAndCreateFileDestinations(args.GetValues("output"));
			
			Splitter.Split(inputs, outputs, partitioner, sort);


			return "Shuffle result.";
		}

	}
}