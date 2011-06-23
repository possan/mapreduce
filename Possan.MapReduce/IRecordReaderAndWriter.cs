namespace Possan.MapReduce
{
	public interface IRecordReaderAndWriter<K, V> : IRecordReader<K,V>, IRecordWriter<K,V>
	{
	}
}