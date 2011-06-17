namespace Possan.MapReduce
{
	public interface IRecordWriter<K, V>
	{
		void Write(K key, V value);
		void Write(IRecordReader<K, V> reader);
	}
}