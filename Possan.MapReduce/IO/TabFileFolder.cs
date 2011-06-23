using System;
using System.Collections.Generic;
using System.IO;

namespace Possan.MapReduce.IO
{
	public class TabFileFolder : IFileSourceAndDestination<string, string>
	{
		private Queue<string> index;
		private string _path;

		public TabFileFolder()
		{
			index = null;
			_path = "";
		}

		public TabFileFolder(string path)
		{
			SetPath(path);
		}


		public void SetPath(string path)
		{
			_path = path;
			index = null;
		}


		// writing

		void EnsurePath()
		{
			if (!Directory.Exists(_path))
				Directory.CreateDirectory(_path);
		}

		public IRecordWriter<string, string> CreateWriter()
		{
			EnsurePath();
			var fn = Guid.NewGuid().ToString().Replace("-", "");
			var p = _path + Path.DirectorySeparatorChar + fn;
			index = null;
			return new TabFileWriter(p);
		}


		// reading

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
			return new TabFileStreamReader(id);
		}
	}
}
