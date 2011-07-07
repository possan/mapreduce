using System.Collections.Generic;

namespace Possan.MapReduce.Distributed
{
	public class DefaultJobArgs : IJobArgs
	{
		private Dictionary<string, List<string>> data;

		public DefaultJobArgs()
		{
			data = new Dictionary<string, List<string>>();
		}

		public void Add(string key, string value)
		{
			if (data.ContainsKey(key))
				data[key].Add(value);
			else
				data.Add(key, new List<string> { value });
		}

		public IEnumerable<string> GetKeys()
		{
			return data.Keys;
		}

		public IList<string> GetValues(string key)
		{
			if (data.ContainsKey(key))
				return data[key];

			return new List<string>();
		}

		public static DefaultJobArgs Parse(IEnumerable<string> args)
		{
			// Dictionary<string,List<string>> ret = new Dictionary<string, List<string>>();
			// return ret;

			var ret = new DefaultJobArgs();
			foreach (var a in args)
			{
				var splitindex = a.IndexOf(":=");
				if (splitindex == -1)
					continue;
				
				ret.Add(a.Substring(0, splitindex), a.Substring(splitindex + 2));
			}

			return ret;
		}

		public static DefaultJobArgs Parse(string[] args)
		{
			return Parse(new List<string>(args));
		}

		public string[] ToArray()
		{
			var ret = new List<string>();
			foreach (var k in data.Keys)
				foreach (var v in data[k])
					ret.Add(k + ":=" + v);
			return ret.ToArray();
		}


	}
}
