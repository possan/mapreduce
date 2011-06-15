namespace Possan.MapReduce
{
	public interface IRecordWriter<K, V>
	{
		bool Write(K key, V value);
	}
}