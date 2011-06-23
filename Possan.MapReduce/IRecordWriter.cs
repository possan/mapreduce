using System;

namespace Possan.MapReduce
{
	public interface IRecordWriter<K, V> : IDisposable
	{
		void Write(K key, V value);
		void Write(IRecordReader<K, V> reader);
		void Write(IRecordStreamReader<K, V> reader);
	}
}