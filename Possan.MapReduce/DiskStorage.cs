using System;
using System.Collections.Generic;
using System.IO;

namespace Possan.MapReduce
{
	public class DiskStorage : IStorage
	{
		string _root = "";

		public DiskStorage(string root)
		{
			_root = root;
		}

		public string CreateFilename()
		{
			return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15) + ".txt";
		}

		public string CreateBatch()
		{
			return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10) + "-batch";
		}

		public IEnumerable<string> GetSubBatches(string parentbatch)
		{
			string path = BuildPath(parentbatch, null);
			var files = Directory.GetDirectories(path);
			// var ret = new List<string>();
			foreach (var f in files)
				yield return Path.GetFileName(f);
		}

		public IEnumerable<string> GetAllFilesInBatch(string batch)
		{
			string path = BuildPath(batch, null);
			var files = Directory.GetFiles(path);
			// var ret = new List<string>();
			foreach (var f in files)
				yield return Path.GetFileName(f);
		}

		public string Get(string batch, string filename)
		{
			string path = BuildPath(batch, filename);
			if (!File.Exists(path))
				return "";
			var content = File.ReadAllText(path);
			return content;
		}

		public void Put(string batch, string filename, string content)
		{
			// Console.WriteLine("Saving to batch \"" + batch + "\" file \"" + filename + "\" (" + content.Length + " characters)");
			string path = BuildPath(batch, filename);
			File.WriteAllText(path, content);
		}
		public void Append(string batch, string filename, string content)
		{
			string path = BuildPath(batch, filename);
			if (File.Exists(path))
				File.AppendAllText(path, content);
			else
				File.WriteAllText(path, content);
		}

		private string BuildPath(string batch, string filename)
		{
			string dir = _root + Path.DirectorySeparatorChar + batch.Replace("/", Path.DirectorySeparatorChar.ToString());
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			if (!string.IsNullOrEmpty(filename))
				dir = dir + Path.DirectorySeparatorChar + filename;
			return dir;
		}

		public void Copy(string inputbatch, string inputfilename, string outputbatch, string outputfilename)
		{
			string inputpath = BuildPath(inputbatch, inputfilename);
			string outputpath = BuildPath(outputbatch, outputfilename);
			File.Copy(inputpath, outputpath, true);
		}

		public void DeleteBatch(string batch)
		{
			string path = BuildPath(batch, null);
			// Console.WriteLine("Deleting path: "+path);
			try
			{
				Directory.Delete(path, true);
			}
			catch(Exception)
			{
				
			}
		}
	}
}
