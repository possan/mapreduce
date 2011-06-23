using System;
using System.IO;

namespace Possan.MapReduce.IO
{
	public class TempFolder : TabFileFolder
	{
		public TempFolder()
		{
			SetPath(Path.GetTempPath() + "mrtemp-" + Guid.NewGuid().ToString());
		}

		public TempFolder(string path)
		{
			SetPath(path);
		}
	}
}