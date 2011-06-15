namespace Possan.MapReduce
{
	public interface IMapperCollector
	{
		void Collect(string key, string value);
	}
}