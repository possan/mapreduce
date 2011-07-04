using System;

namespace Possan.MapReduce.IO
{
	public class DummyFileSource : IFileSource<string, string>
	{
		private readonly int _recordsperfile;
		private int _recordsleft;
		private readonly string _dummykey;
		private readonly string _dummyvalue;
		private int counter;

		public DummyFileSource(int totalrecordsfiles, int maxrecordsperfile, string dummykey, string dummyvalue)
		{
			_recordsleft = totalrecordsfiles;
			_recordsperfile = maxrecordsperfile;
			_dummykey = dummykey;
			_dummyvalue = dummyvalue;
		}

		public bool ReadNext(out string id)
		{
			int filesInBatch = Math.Min(_recordsleft, _recordsperfile);
			if (filesInBatch > 0)
			{
				id = "filebatch," + counter + "," + filesInBatch;
				_recordsleft -= filesInBatch;
				counter++;
				return true;
			}
			id = "";
			return false;
		}

		public IRecordStreamReader<string, string> CreateStreamReader(string id)
		{
			int filesInBatch = int.Parse(id.Split(',')[2]);
			return new DummyRecordStreamReader(_dummykey, _dummyvalue, filesInBatch);
		}
	}
}