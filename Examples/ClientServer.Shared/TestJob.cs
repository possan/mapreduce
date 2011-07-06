using System;
using Possan.Distributed;

namespace ClientServer.Shared
{
	public class TestJob : ISandboxedJob
	{
		public string Run(string[] args)
		{
			Console.WriteLine("TestJob called, args: " + string.Join(", ", args));
			return "";
		}
	}
}
