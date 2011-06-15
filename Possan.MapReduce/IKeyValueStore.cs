namespace Possan.MapReduce
{
	public interface IKeyValueStore
	{
		string Get(string key, string defaultvalue);
		void Put(string key, string value);
	}
}
