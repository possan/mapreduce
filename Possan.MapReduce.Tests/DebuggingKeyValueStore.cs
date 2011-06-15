using System;

namespace Possan.MapReduce.Tests
{
	class DebuggingKeyValueStore : IKeyValueStore
	{
		private readonly IKeyValueStore _real;

		public DebuggingKeyValueStore(IKeyValueStore real)
		{
			_real = real;
		}


		public string Get(string key, string defaultvalue)
		{
			var realvalue = _real.Get(key, defaultvalue);
			Console.WriteLine("Read key " + key + ", got " + realvalue);
			return realvalue;
		}

		public void Put(string key, string value)
		{
			Console.WriteLine("Put key " + key + ", value " + value);
			_real.Put(key, value);
		}
	}
}