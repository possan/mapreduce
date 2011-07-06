using System;
using Possan.MapReduce;

namespace ClientServer.Shared
{
	public class RandomPointInCircleMapper : IMapper
	{
		public void Map(string key, string value, IMapperCollector collector)
		{
			var r = new Random();
			var x = -1.0 + (r.NextDouble() * 2.0);
			var y = -1.0 + (r.NextDouble() * 2.0);
			var d = x * x + y * y;
			if (d >= 1.0)
				return;
			collector.Collect("x", "1");
		}
	}
	
}
