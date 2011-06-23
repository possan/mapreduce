using System;
using System.Collections.Generic;
using Possan.MapReduce;

namespace mrtest
{
	class TestReducer : IReducer
	{
		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector, bool isrereduce)
		{
			int n = 0;
			foreach (var value in values)
				n += int.Parse(value.ToString());
			if (n >= 1)
				collector.Collect(key, "" + n);
		}
		 
	}
}
