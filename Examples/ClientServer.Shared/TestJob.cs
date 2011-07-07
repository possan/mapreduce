using System;
using Possan.Distributed;

namespace ClientServer.Shared
{
	public class TestJob : ISandboxedJob
	{
		public string Run(IJobArgs args)
		{
			Console.WriteLine("TestJob called");
			return "";
		}
	}
}
