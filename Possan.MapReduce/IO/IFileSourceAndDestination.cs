namespace Possan.MapReduce.IO
{
	public interface IFileSourceAndDestination<K,V> : IFileDestination<K, V>, IFileSource<K, V>
	{
		
	}
}