namespace Possan.MapReduce
{
	public interface IFileDestination<K, V>
	{
		// bool ReadNext(out string id);
		IRecordWriter<K, V> CreateWriter();
	}
}