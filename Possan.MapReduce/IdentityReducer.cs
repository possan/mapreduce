using System.Collections.Generic;

namespace Possan.MapReduce
{
	public class IdentityReducer : IReducer
	{
		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector)
		{
			foreach (var value in values)
				collector.Collect(key, value);
		}
	}
}
