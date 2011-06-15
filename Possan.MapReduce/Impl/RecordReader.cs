using System.Collections.Generic;

namespace Possan.MapReduce.Impl
{
	public class RecordReader : IRecordReader<string, string>
	{
		private readonly IStorage _storage;
		private readonly string _batch;

		public RecordReader(IStorage storage, string batch)
		{
			_storage = storage;
			_batch = batch;
		}

		public IEnumerable<string> GetKeys()
		{
			// _storage.get
			var dirs = _storage.GetSubBatches(_batch);
			foreach (var d in dirs)
			{
				yield return d;
			}
		}

		public IEnumerable<string> GetValues(string key)
		{
			var files = _storage.GetAllFilesInBatch(_batch + "/" + key);
			foreach (var d in files)
			{
				var data = _storage.Get(_batch + "/" + key, d);
				yield return data;
			}
		}
	}
}