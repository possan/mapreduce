using System.Collections.Generic;

namespace Possan.MapReduce
{
	public class GroupValuesByKeyReducer : IReducer
	{
		private readonly IComparer<string> _comparer;
		private readonly string _separator;

		public GroupValuesByKeyReducer()
		{
			_separator = ",";
			_comparer = null;
		}

		public GroupValuesByKeyReducer(string separator)
		{
			_separator = separator;
			_comparer = null;
		}

		public GroupValuesByKeyReducer(string separator, IComparer<string> comparer)
		{
			_separator = separator;
			_comparer = comparer;
		}

		public void Reduce(string key, IEnumerable<string> values, IReducerCollector collector)
		{
			var tmp = new List<string>(values);
			if( _comparer != null )
				tmp.Sort(_comparer);
			else
				tmp.Sort();
			collector.Collect(key, string.Join(_separator, tmp.ToArray()));
		}
	}
}