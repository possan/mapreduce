namespace Possan.MapReduce.Mappers
{
	public class SwapKeyValueMapper : IMapper
	{
		public void Map(string key, string value, IMapperCollector collector)
		{
			collector.Collect(value, key);
		}
	}
}