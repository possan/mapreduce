using System;
using System.Collections.Generic;

namespace Possan.MapReduce.IO
{
	public class InMemoryTempFolder : IFileSourceAndDestination<string, string>
	{
		private MemoryKeyValueStreamReaderWriter buffer;
		private Queue<string> readqueue;

		public IRecordStreamReaderAndWriter<string, string> Buffer
		{
			get
			{
				return buffer;
			}
		}

		public InMemoryTempFolder()
		{
			buffer = new MemoryKeyValueStreamReaderWriter();
			readqueue = null;
		}

		void EnsureReadCache()
		{
			if (readqueue != null)
				return;
			readqueue = new Queue<string>();
			readqueue.Enqueue("all");
		}

		public IRecordWriter<string, string> CreateWriter()
		{
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
		}
	}

}