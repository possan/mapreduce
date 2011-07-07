using System.Collections.Generic;

namespace Possan.MapReduce.Distributed
{
	public interface IJobArgs
	{
		void Add(string key, string value);
		IEnumerable<string> GetKeys();
		IList<string> GetValues(string key);
		string[] ToArray();
	}
}