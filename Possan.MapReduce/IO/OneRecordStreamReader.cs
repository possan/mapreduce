namespace Possan.MapReduce.IO
{
	public class DummyRecordStreamReader : IRecordStreamReader<string, string>
	{
		private readonly string _key;
		private readonly string _value;
		private int _read;
		private int _total;

		public DummyRecordStreamReader(string key, string value, int count)
		{
			_key = key;
			_value = value;
			_total = count;
			_read = 0;
		}

		public bool Read(out string key, out string value)
		{
			if (_read >= _total)
			{
				key = "";
				value = "";
				return false;
			}
			key = _key;
			value = _value;
			_read++;
			return true;
		}

		public void Dispose()
		{
		}
	}
}