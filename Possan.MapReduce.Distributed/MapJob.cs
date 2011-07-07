using System;
using Possan.Distributed;
using Possan.MapReduce.Util;

namespace Possan.MapReduce.Distributed
{
	public class MapJob : ISandboxedJob
	{
		public string Run(IJobArgs args)
		{
			Console.WriteLine("Inside map job");

			string mappertype = args.Get("mapper");
			
			var mapper = Activator.CreateInstance(Type.GetType(mappertype)) as IMapper;
			var reader = JobUtilities.ParseAndCreateFileSource(args.Get("input"));
			var writer = JobUtilities.ParseAndCreateFileDestination(args.Get("output")); 

			Mapper.Map(reader, writer, mapper);

			return "Map result.";
		}
	}
}