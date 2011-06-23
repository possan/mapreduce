using System;
using System.Collections.Generic;
using System.IO;
using Possan.MapReduce;
using Possan.MapReduce.IO;

namespace ArticleKeywords.IO
{
	public class TextFilesFolderSource : IFileSource<string, string>
	{
		private readonly string _path;
		private Queue<string> index;

		public TextFilesFolderSource(string path)
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
			if (index != null)
				return;

			index = new Queue<string>();

			Console.WriteLine("Buffering files in " + _path);

			var files = Directory.GetFiles(_path);
			foreach (var file in files)
				index.Enqueue(file);

			Console.WriteLine("Done.");
		}

		public IRecordStreamReader<string, string> CreateStreamReader(string id)
		{
			var key = Path.GetFileName(id);
			return new TextFileLinesStreamReader(id, key);
		}
	}
}