using System;
using Possan.Distributed;

namespace Possan.MapReduce.Distributed
{
	public class ReduceJob : ISandboxedJob
	{
		public string Run(string[] args)
		{
			Console.WriteLine("Inside reduce job");
			return "Reduce result.";
		}
	}
}