using System;

namespace Possan.MapReduce
{
	public interface IRecordStreamReader<K, V> : IDisposable
	{
		bool Read(out K key, out V value);
	}
}