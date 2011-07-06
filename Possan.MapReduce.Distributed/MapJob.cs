using System;
using Possan.Distributed;
using Possan.MapReduce.Util;

namespace Possan.MapReduce.Distributed
{
	public class MapJob : ISandboxedJob
	{
		public string Run(string[] args)
		{
			Console.WriteLine("Inside map job");

			string mappertype = args[0];
			string inputtype = args[1];
			string outputtype = args[2];
			string inputfolder = args[3];
			string outputfolder = args[4];
			
			/*
			var outputwriters = new List<IFileDestination<string, string>>();

			Console.WriteLine("Partition using: " + partitionertype);
			Console.WriteLine("Partition from folder: " + inputfolder + " (of type " + inputtype + ")");
			Console.WriteLine("Partition into " + outputs.Count + " buckets: (of type " + outputtype + ")");
			foreach (var o in outputs)
			{
				Console.WriteLine("  " + o);
				// var w = new TabFileFolderWriter(o);
				var writer = Activator.CreateInstance(Type.GetType(outputtype), new object[] { o }) as IFileDestination<string, string>;
				outputwriters.Add(writer);
			}
			*/

			var mapper = Activator.CreateInstance(Type.GetType(mappertype)) as IMapper;

			var reader = Activator.CreateInstance(Type.GetType(inputtype), new object[] { inputfolder }) as IFileSource<string, string>;
			var writer = Activator.CreateInstance(Type.GetType(outputtype), new object[] { outputfolder }) as IFileDestination<string, string>;

			Mapper.Map(reader, writer, mapper);

			return "Map result.";
		}
	}
}