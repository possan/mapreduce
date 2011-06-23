namespace Possan.MapReduce
{
	public interface IRecordStreamReaderAndWriter<K, V> : IRecordStreamReader<K, V>, IRecordWriter<K, V>
	{
	}
}