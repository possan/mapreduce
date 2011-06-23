using System.Collections.Generic;

namespace Possan.MapReduce
{ 
	public interface IReducer
	{ 
		void Reduce(string key, IEnumerable<string> values, IReducerCollector collector, bool isrereduce);
	}
}