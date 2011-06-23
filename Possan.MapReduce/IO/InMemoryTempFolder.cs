using System;
using System.Collections.Generic;

namespace Possan.MapReduce.IO
{
	public class InMemoryTempFolder : IFileSourceAndDestination<string, string>
	{
	//	private Dictionary<string, IRecordStreamReaderAndWriter<string, string>> tempfiles;
		private MemoryKeyValueStreamReaderWriter buffer;
		private Queue<string> readqueue;

		public InMemoryTempFolder()
		{
			buffer = new MemoryKeyValueStreamReaderWriter();
			// tempfiles = new Dictionary<string, IRecordStreamReaderAndWriter<string,string>>();
			readqueue = null;
		}

		void EnsureReadCache()
		{
			if (readqueue != null)
				return;

			readqueue = new Queue<string>();
			readqueue.Enqueue("all");
			//	foreach(var f in tempfiles.Keys)
			//		readqueue.Enqueue(f);
		}

		public IRecordWriter<string, string> CreateWriter()
		{
			//	var fn = Guid.NewGuid().ToString();
			//	var buf = new MemoryKeyValueStreamReaderWriter();
			//	tempfiles.Add(fn, buf );
			readqueue = null;
			return buffer;
		}

		public bool ReadNext(out string id)
		{
			EnsureReadCache();

			id = "";
			if (readqueue.Count < 1)
				return false;

			id = readqueue.Dequeue();
			return true;
		}

		public IRecordStreamReader<string, string> CreateStreamReader(string id)
		{
			if (id != "all")
				return null;
			
			return buffer;
			// return tempfiles[id];
			// throw new NotImplementedException();
		}
	}

}