using System;
using System.Collections.Generic;

namespace Possan.MapReduce
{
	public interface IRecordReader<K, V> : IDisposable
	{ 
		IEnumerable<K> GetKeys();
		IEnumerable<V> GetValues(K key);
	}
}