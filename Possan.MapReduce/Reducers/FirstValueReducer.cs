using System.Collections.Generic;
using System.Linq;

namespace Possan.MapReduce.Reducers
{
	public class FirstValueReducer : IReducer
	{
		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector, bool isrereduce)
		{
			collector.Collect(key, values.First());
		}
	}
}
