using System;
using System.Collections.Generic;

namespace Possan.MapReduce.IO
{
	public class MemoryKeyValueReaderWriter : IRecordReaderAndWriter<string, string>
	{
		private Dictionary<string, List<string>> _data;
		private int _counter;

		public MemoryKeyValueReaderWriter()
		{
			_data = new Dictionary<string, List<string>>();
			_counter = 0;
		}

		public int Count
		{
			get { return _counter; }
		}

		public IEnumerable<string> GetKeys()
		{
			var list = new List<string>(_data.Keys);
			list.Sort();
			return list;
		}

		public IEnumerable<string> GetValues(string key)
		{
			if (key == null)
				return new List<string>();

			if (!_data.ContainsKey(key))
				return new List<string>();

			var list = _data[key];
			list.Sort();
			return list;
		}

		void InLockAdd(string key, string value, bool checkkey)
		{
			if (string.IsNullOrEmpty(key))
				return;
			if (string.IsNullOrEmpty(value))
				return;
			if (checkkey && !_data.ContainsKey(key))
				_data.Add(key, new List<string>());
			_data[key].Add(value);
			_counter++;
			if (_counter % 20000 == 0 && _counter > 0)
			{
				Console.WriteLine("Memory key/value store; added key #" + _counter);
			}
		}

		public void Write(string key, string value)
		{
			lock (_data)
			{
				InLockAdd(key, value, true);
			}
		}

		public void Write(IRecordStreamReader<string, string> reader)
		{
			lock (_data)
			{
				string k;
				string v;
				while (reader.Read(out k, out v))
				{
					InLockAdd(k, v, true);
				}
			}
		}

		public void Write(IRecordReader<string, string> reader)
		{
			lock (_data)
			{
				foreach (var k in reader.GetKeys())
				{
					if (k == null)
						continue;
					if (!_data.ContainsKey(k))
						_data.Add(k, new List<string>());
					foreach (var v in reader.GetValues(k))
					{
						InLockAdd(k, v, false);
					}
				}
			}
		}

		public void Dispose()
		{
			// throw new NotImplementedException();
		}
	}
}
