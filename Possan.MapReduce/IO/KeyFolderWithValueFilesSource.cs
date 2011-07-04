using System;
using System.Collections.Generic;
using System.IO;

namespace Possan.MapReduce.IO
{
	public class KeyFolderWithValueFilesSource : IFileSource<string, string>
	{
		private readonly string _root;
		private Queue<string> index;

		public KeyFolderWithValueFilesSource(string root)
		{
			_root = root;
			index = null;
			// keyfolders = Directory.GetFiles(root);
		}

		public bool ReadNext(out string id)
		{
			ensureCache();

			id = "";
			if (index.Count < 1)
				return false;

			id = index.Dequeue();
			return true;
		}

		private void ensureCache()
		{
			if( index != null )
				return;
			
			index = new Queue<string>();

			Console.WriteLine("Buffering files in " + _root);
				
			var folders = Directory.GetDirectories(_root);
			foreach (var folder in folders)
			{
				var files = Directory.GetFiles(folder);
				foreach (var file in files)
				{
					index.Enqueue(file);
				}
			}

			Console.WriteLine("Done.");
		}

		public IRecordStreamReader<string, string> CreateStreamReader(string id)
		{
			string key = Path.GetFileName( Path.GetDirectoryName(id));
			string value = File.ReadAllText(id);
			return new DummyRecordStreamReader(key, value,1);
		} 
	}
}