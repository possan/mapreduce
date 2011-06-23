using System;
using System.Collections.Generic;
using System.IO;

namespace Possan.MapReduce.IO
{
	public class TabFileFolderSource : IFileSource<string, string>
	{
		private readonly string _path;
		private Queue<string> index;

		public TabFileFolderSource(string path)
		{
			_path = path;
			index = null;
		}
		
		public bool ReadNext(out string id)
		{
			EnsureCache();

			id = "";
			if (index.Count < 1)
				return false;

			id = index.Dequeue();
			return true;
		}

		void EnsureCache()
		{
			if( index != null )
				return;

			index = new Queue<string>();

			Console.WriteLine("Buffering files in " + _path);
				
			var files = Directory.GetFiles(_path);
			foreach(var file in files)
				index.Enqueue(file);

			Console.WriteLine("Done.");
		}

		public IRecordStreamReader<string, string> CreateStreamReader(string id)
		{
			return new TabFileStreamReader(id);
		}
	}
}