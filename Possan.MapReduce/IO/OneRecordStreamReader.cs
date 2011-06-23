namespace Possan.MapReduce.IO
{
	public class OneRecordStreamReader : IRecordStreamReader<string, string>
	{
		private readonly string _key;
		private readonly string _value;
		private bool hasBeenRead;

		public OneRecordStreamReader(string key, string value)
		{
			_key = key;
			_value = value;
			hasBeenRead = false;
		}

		public bool Read(out string key, out string value)
		{
			key = "";
			value = "";
			if (hasBeenRead)
				return false;
			key = _key;
			value = _value;
			hasBeenRead = true;
			return true;
		}

		public void Dispose()
		{
		}
	}
}