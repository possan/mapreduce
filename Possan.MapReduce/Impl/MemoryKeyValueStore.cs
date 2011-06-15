using System.Collections.Generic;

namespace Possan.MapReduce.Impl
{
	public class MemoryKeyValueStore : IKeyValueStore
	{
		private Dictionary<string, string> _data;

		public MemoryKeyValueStore()
		{
			_data = new Dictionary<string, string>();
		}

		public string Get(string key, string defaultvalue)
		{
			if (_data.ContainsKey(key))
				return _data[key];
			return defaultvalue;
		}

		public void Put(string key, string value)
		{
			if (_data.ContainsKey(key))
				_data[key] = value;
			else
				_data.Add(key, value);
		}
	}
}
