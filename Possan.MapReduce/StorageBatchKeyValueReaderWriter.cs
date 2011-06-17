using System.Collections.Generic;

namespace Possan.MapReduce
{
	public class StorageBatchKeyValueReaderWriter : IRecordReader<string, string>, IRecordWriter<string, string>
	{
		private readonly IStorage _storage;
		private readonly string _batch;

		public StorageBatchKeyValueReaderWriter(IStorage storage, string batch)
		{
			_storage = storage;
			_batch = batch;
		}

		public IEnumerable<string> GetKeys()
		{
			return _storage.GetSubBatches(_batch);
		}

		public IEnumerable<string> GetValues(string key)
		{
			var values = new List<string>();
			if (key != null)
			{
				var files = _storage.GetAllFilesInBatch(_batch + "/" + key);
				foreach (var f in files)
					values.Add(_storage.Get(_batch + "/" + key, f));
			}
			return values;
		}

		public void Write(string key, string value)
		{
			if (key == null)
				return;

			if (value == null)
				return;

			_storage.Put(_batch + "/" + key, _storage.CreateFilename(), value);
		}

		public void Write(IRecordReader<string, string> reader)
		{
			foreach (var k in reader.GetKeys())
				foreach (var v in reader.GetValues(k))
					Write(k, v);
		}
	}
}