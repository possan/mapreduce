using System.Collections.Generic;

namespace Possan.MapReduce
{
	public class NonLockingMemoryKeyValueReaderWriter : IRecordReader<string, string>, IRecordWriter<string, string>
	{
		private Dictionary<string, List<string>> _data;

		public NonLockingMemoryKeyValueReaderWriter()
		{
			_data = new Dictionary<string, List<string>>();
		}

		public IEnumerable<string> GetKeys()
		{
			var list = new List<string>(_data.Keys);
			list.Sort();
			return list;
		}

		public IEnumerable<string> GetValues(string key)
		{
			if (key != null)
			{
				if (_data.ContainsKey(key))
				{
					var list = _data[key];
					list.Sort();
					return list;
				}
			}
			return new List<string>();
		}

		public void Write(string key, string value)
		{
			if (!_data.ContainsKey(key))
				_data.Add(key, new List<string>());
			_data[key].Add(value);
		}

		public void Write(IRecordReader<string, string> reader)
		{
			foreach (var k in reader.GetKeys())
			{
				if (k == null)
					continue;
				if (!_data.ContainsKey(k))
					_data.Add(k, new List<string>());
				foreach (var v in reader.GetValues(k))
				{
					if (v == null)
						continue;
					_data[k].Add(v);
				}
			}
		}
	}
}