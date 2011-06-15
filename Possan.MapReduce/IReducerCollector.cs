namespace Possan.MapReduce
{
	public interface IReducerCollector 
	{
		void Collect(string key, string value);
	}
}