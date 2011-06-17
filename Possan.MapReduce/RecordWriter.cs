using System;

namespace Possan.MapReduce
{
	public class RecordWriter : IRecordWriter<string, string>
	{
		private readonly IStorage _storage;
		private readonly string _batch;

		public RecordWriter(IStorage storage, string batch)
		{
			_storage = storage;
			_batch = batch;
		}

		public void Write(string key, string value)
		{
			if (key == null)
				return;

			if (value == null)
				return;

			string dir = _batch + "/" + key;
			_storage.Put(dir, Guid.NewGuid().ToString().Replace("-", "") + ".txt", value);
		}

		public void Write(IRecordReader<string, string> reader)
		{
			foreach (var k in reader.GetKeys())
				foreach (var v in reader.GetValues(k))
					Write(k, v);
		}
	}
}