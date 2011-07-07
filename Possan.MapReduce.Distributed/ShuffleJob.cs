using System;
using Possan.Distributed;
using Possan.MapReduce.Util;

namespace Possan.MapReduce.Distributed
{
	public class ShuffleJob : ISandboxedJob
	{
		public string Run(IJobArgs args)
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