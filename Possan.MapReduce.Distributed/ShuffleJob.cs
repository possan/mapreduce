using System;
using Possan.Distributed;

namespace Possan.MapReduce.Distributed
{
	public class ShuffleJob : ISandboxedJob
	{
		public string Run(string[] args)
		{
			Console.WriteLine("Inside shuffle job");
			return "Shuffle result.";
		}
	}
}