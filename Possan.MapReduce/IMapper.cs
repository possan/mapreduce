namespace Possan.MapReduce
{
	public interface IMapper
	{
		void Map(string key, string value, IMapperCollector collector);
	}
}
