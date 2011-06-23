using System;
using System.Collections.Generic;
using System.Linq;

namespace Possan.MapReduce.IO
{
	public class GenericKeyValueStorage : IStorage
	{
		private readonly IKeyValueStore _backend;

		public GenericKeyValueStorage(IKeyValueStore backend)
		{
			_backend = backend;
		}

		public string CreateFilename()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}

		public string CreateBatch()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}

		string getfoldername(string folder)
		{
			var batchsteps = folder.Split('/');
			return batchsteps[batchsteps.Length - 1];
		}

		string getparentfolder(string folder)
		{
			var batchsteps = folder.Split('/');
			return string.Join("/", batchsteps, 0, batchsteps.Length - 1);
		}

		bool folderexists(string folder)
		{
			var parentfolder = getparentfolder(folder);
			var foldername = getfoldername(folder);
			if (parentfolder == "")
				return true;

			var psf = getsubfolders(parentfolder);
			return psf.Contains(foldername);
		}

		void _ensurefolder(string parentpath, string foldername)
		{
			// Console.WriteLine("Ensure folder " + foldername + " in " + parentpath);

			if (parentpath == "")
				return;

			var list = getsubfolders(parentpath);
			if (list.Contains(foldername))
				return;

			list.Add(foldername);
			PutList(getsubfolderkey(parentpath), list);
		}


		void ensurefolders(string folderpath)
		{
			if (folderexists(folderpath))
				return;

			// Console.WriteLine("Ensure folders " + folderpath);
			var batchsteps = folderpath.Split('/');
			for (int k = 1; k <= batchsteps.Length - 1; k++)
			{
				var thispath = string.Join("/", batchsteps, 0, k);
				var nextpath = batchsteps[k];
				_ensurefolder(thispath, nextpath);
			}
		}

		string getsubfolderkey(string folder)
		{
			return "folders_in_folder__" + folder;
		}

		string getfolderfileskey(string folder)
		{
			return "files_in_folder__" + folder;
		}

		IList<string> getsubfolders(string folder)
		{
			if (!folderexists(folder))
				return new List<string>();

			string key = getsubfolderkey(folder);
			return GetList(key);
		}

		IList<string> getfolderfiles(string folder)
		{
			if (!folderexists(folder))
				return new List<string>();
			string key = getfolderfileskey(folder);
			return GetList(key);
		}

		void addfolder(string parentfolder, string newfolder)
		{
			if( newfolder == "" )
				return;
			ensurefolders(parentfolder);
			string key = getsubfolderkey(parentfolder);
			var list = getsubfolders(parentfolder);
			if( list.Contains(newfolder))
				return;
			list.Add(newfolder);
			PutList(key, list);
		}

		void addfile(string parentfolder, string newfile)
		{
			if (newfile == "")
				return;
			// Console.WriteLine("addig file " + newfile + " to " + parentfolder);
			ensurefolders(parentfolder);
			string key = getfolderfileskey(parentfolder);
			var list = getfolderfiles(parentfolder);
			if(list.Contains(newfile))
				return;
			list.Add(newfile);
			PutList(key, list);
		}

		public void InvalidateBatch(string batch)
		{
			string key = getfolderfileskey(batch);
			PutList(key, new List<string>());

			key = getsubfolderkey(batch);
			PutList(key, new List<string>());
		}

		/*
		string GetBatchPrefix(string batch)
		{
			var batchsteps = batch.Split('/');
			foreach (var batchstep in batchsteps)
			{
				string parentprefix = "";

			}

			string prefixkey = "batch_" + batch + "_prefix";

			string batchprefix = _backend.Get(prefixkey, "-");
			if (batchprefix != "-")
				return batchprefix;

			var newprefix = Guid.NewGuid().ToString().Replace("-", "");
			_backend.Put(prefixkey, newprefix);
			return newprefix;
		}
		*/











		IList<string> GetList(string key)
		{
			string data = _backend.Get(key, "");
			string[] list = data.Split('|');
			return new List<string>(list).Where(p=>!string.IsNullOrEmpty(p)).ToList();
		}

		void PutList(string key, IList<string> values)
		{
			var newlist = values.Where(p => !string.IsNullOrEmpty(p)).ToList();
			newlist.Sort();
			// newlist = newlist.OrderBy((a, b) => { return a.CompareTo(b); });
			string list = string.Join("|", newlist.ToArray());
			_backend.Put(key, list);
		}
















		public void DeleteBatch(string batch)
		{
			// Console.WriteLine("DeleteBatch(" + batch + ");");
			InvalidateBatch(batch);
		}

		public IEnumerable<string> GetSubBatches(string parentbatch)
		{
			return new List<string>(getsubfolders(parentbatch));
		}

		public IEnumerable<string> GetAllFilesInBatch(string batch)
		{
			return new List<string>(getfolderfiles(batch));
		}

		public string Get(string batch, string filename)
		{
			var files = getfolderfiles(batch);
			if (!files.Contains(filename))
			{
				// Console.WriteLine("file " + filename + " not found within " + batch);
				return "";
			}

			var datakey = batch + "____" + filename;
			return _backend.Get(datakey, "");
		}

		public void Put(string batch, string filename, string content)
		{
			addfile(batch, filename);
			var datakey = batch + "____" + filename;
			_backend.Put(datakey, content);
		}

		public void Copy(string inputbatch, string inputfilename, string outputbatch, string outputfilename)
		{
			Put(outputbatch, outputfilename, Get(inputbatch, inputfilename));
		}
	}
}
