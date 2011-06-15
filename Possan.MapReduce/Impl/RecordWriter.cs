using System;

namespace Possan.MapReduce.Impl
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

		public bool Write(string key, string value)
		{
			// Console.WriteLine("key: " + key + " value: " + value);
			string dir = _batch + "/" + key;
			_storage.Put(dir, Guid.NewGuid().ToString().Replace("-", "") + ".txt", value);
			return true;
		}
	}
}