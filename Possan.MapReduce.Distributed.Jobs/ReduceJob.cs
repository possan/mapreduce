using System;
using Possan.Distributed.Sandbox;
using Possan.MapReduce.Util;

namespace Possan.MapReduce.Distributed.Jobs
{
	public class ReduceJob : ISandboxedJob
	{
		public string Run(ISandboxedJobArgs args)
		{
			Console.WriteLine("Inside reduce job");

			string reducertype = args.Get("reducer");
			var reducer = Activator.CreateInstance(Type.GetType(reducertype)) as IReducer;

			var reader = JobUtilities.ParseAndCreateFileSource(args.Get("input"));
			
			var writer = JobUtilities.ParseAndCreateFileDestination(args.Get("output"));

			Reducer.Reduce(reader, writer, reducer);
			
			return "Reduce result.";
		}
	}
}