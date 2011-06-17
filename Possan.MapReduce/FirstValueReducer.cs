using System.Collections.Generic;
using System.Linq;

namespace Possan.MapReduce
{
	public class FirstValueReducer : IReducer
	{
		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector)
		{
			collector.Collect(key, values.First());
		}
	}
}
