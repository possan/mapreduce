using System.Collections.Generic;

namespace Possan.MapReduce.Reducers
{
	public class IdentityReducer : IReducer
	{
		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector, bool isrereduce)
		{
			foreach (var value in values)
				collector.Collect(key, value);
		}
	}
}
