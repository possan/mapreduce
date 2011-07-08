using System;
using Possan.Distributed.Sandbox;
using Possan.MapReduce;

namespace ClientServer.Shared
{
	public class TestJob : ISandboxedJob
	{
		public string Run(ISandboxedJobArgs args)
		{
			Console.WriteLine("TestJob called.");
			IMapper tmp = new TestMapper();
			tmp.Map("x", "apa bpa cpa dpa", new DummyCollector());
			Console.WriteLine("TestJob done.");
			return "";
		}
		
		public class DummyCollector : IMapperCollector
		{
			public void Collect(string key, string value)
			{
				Console.WriteLine("DummyCollector collected " + key + " => " + value);
			}
		}
	}
}
