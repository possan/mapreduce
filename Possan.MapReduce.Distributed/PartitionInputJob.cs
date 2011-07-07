using System;
using System.Collections.Generic;
using Possan.Distributed;
using Possan.MapReduce.Util;

namespace Possan.MapReduce.Distributed
{
	public class PartitionInputJob : ISandboxedJob
	{
		public string Run(string[] args)
		{
			Console.WriteLine("Inside partition input job");

			string partitionertype = args[0];
			string inputtype = args[1];
			string outputtype = args[2];
			string inputfolder = args[3];
			var outputs = new List<string>();
			for (int i = 4; i < args.Length; i++)
				outputs.Add(args[i]);

			var outputwriters = new List<IFileDestination<string, string>>();

			Console.WriteLine("Partition using: " + partitionertype);
			Console.WriteLine("Partition from folder: " + inputfolder + " (of type " + inputtype + ")");
			Console.WriteLine("Partition into " + outputs.Count + " buckets: (of type " + outputtype + ")");
			foreach (var o in outputs)
			{
				Console.WriteLine("  " + o);
				var writer = Activator.CreateInstance(Type.GetType(outputtype), new object[] { o }) as IFileDestination<string, string>;
				outputwriters.Add(writer);
			}

			var partitioner = Activator.CreateInstance(Type.GetType(partitionertype)) as IShardingPartitioner;
			var inputreader = Activator.CreateInstance(Type.GetType(inputtype), new object[] { inputfolder }) as IFileSource<string, string>;
			var inputs = new List<IFileSource<string, string>>();
			inputs.Add(inputreader);
			Splitter.Split(inputs, outputwriters, partitioner, false);

			return "Partition result.";
		}
	}
}