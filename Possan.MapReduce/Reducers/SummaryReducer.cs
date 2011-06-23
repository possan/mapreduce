using System.Collections.Generic;
using System.Globalization;

namespace Possan.MapReduce.Reducers
{
	public class SummaryReducer : IReducer
	{
		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector, bool isrereduce)
		{
			float sum = .0f;
			foreach (var value in values)
			{
				float t = .0f;
				float.TryParse(value, NumberStyles.AllowDecimalPoint|NumberStyles.Number, CultureInfo.InvariantCulture, out t);
				sum += t;
			}
			collector.Collect(key, sum.ToString(CultureInfo.InvariantCulture));
		}
	}
}