namespace Possan.MapReduce
{
	public interface IFileSource<K, V>
	{
		bool ReadNext(out string id);
		IRecordStreamReader<K, V> CreateStreamReader(string id);
	}
}