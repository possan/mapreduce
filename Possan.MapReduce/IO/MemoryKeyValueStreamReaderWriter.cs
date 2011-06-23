using System;
using System.Collections.Generic;

namespace Possan.MapReduce.IO
{
	public class MemoryKeyValueStreamReaderWriter : IRecordStreamReaderAndWriter<string, string>
	{
		private Dictionary<string, List<string>> _data;
		private int _counter;
		private Queue<KeyValuePair<string, string>> _readqueue;

		public MemoryKeyValueStreamReaderWriter()
		{
			_data = new Dictionary<string, List<string>>();
			_readqueue = null;
			_counter = 0;
		}

		public int Count
		{
			get { return _counter; }
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
			_readqueue = null;
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

		public bool Read(out string key, out string value)
		{
			EnsureReadQueue();

			key = "";
			value = "";
			if (_readqueue.Count < 1)
				return false;

			var item = _readqueue.Dequeue();
			key = item.Key;
			value = item.Value;
			return true;
		}

		private void EnsureReadQueue()
		{
			if (_readqueue != null)
				return;

			_readqueue = new Queue<KeyValuePair<string, string>>();
			foreach (var key in _data.Keys)
				foreach (var value in _data[key])
					_readqueue.Enqueue(new KeyValuePair<string, string>(key, value));
		}

		public void Dispose()
		{
			// throw new NotImplementedException();
		}
	}
}