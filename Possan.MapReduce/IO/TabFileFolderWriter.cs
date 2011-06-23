using System;
using System.IO;

namespace Possan.MapReduce.IO
{
	public class TabFileFolderWriter : IFileDestination<string, string>
	{
		private readonly string _path;

		public TabFileFolderWriter(string path)
		{
			_path = path;
			if (!Directory.Exists(_path))
				Directory.CreateDirectory(_path);
		}

		public IRecordWriter<string, string> CreateWriter()
		{
			var fn = Guid.NewGuid().ToString().Replace("-", "");
			var p = _path + Path.DirectorySeparatorChar + fn;
			return new TabFileWriter(p);
		}
	}
}