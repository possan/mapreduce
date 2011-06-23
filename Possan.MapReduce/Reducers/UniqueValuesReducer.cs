using System.Collections.Generic;

namespace Possan.MapReduce.IO
{
	public class UniqueValuesReducer : IReducer
	{
		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector, bool isrereduce)
		{
			var arch = new List<string>();
			foreach (var value in values)
			{
				if( arch.Contains(value))
					continue;
				collector.Collect(key, value);
				arch.Add(value);
			}
		}
	}
}