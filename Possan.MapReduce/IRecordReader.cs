using System.Collections.Generic;

namespace Possan.MapReduce
{
	public interface IRecordReader<K, V>
	{ 
		IEnumerable<K> GetKeys();
		IEnumerable<V> GetValues(K key);
	}
}