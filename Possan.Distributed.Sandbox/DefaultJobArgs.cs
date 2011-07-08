using System;
using System.Collections.Generic;

namespace Possan.Distributed.Sandbox
{
	public class SandboxedJobArgs : MarshalByRefObject, ISandboxedJobArgs
	{
		private Dictionary<string, List<string>> data;

		public SandboxedJobArgs()
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

		public string Get(string key)
		{
			return Get(key, "");
		}

		public string Get(string key, string defaultvalue)
		{
			if (data.ContainsKey(key))
				if (data[key].Count > 0)
					return data[key][0];
			return defaultvalue;
		}

		public IList<string> GetValues(string key)
		{
			if (data.ContainsKey(key))
				return data[key];

			return new List<string>();
		}

		public static SandboxedJobArgs Parse(IEnumerable<string> args)
		{
			// Dictionary<string,List<string>> ret = new Dictionary<string, List<string>>();
			// return ret;

			var ret = new SandboxedJobArgs();
			foreach (var a in args)
			{
				var splitindex = a.IndexOf(":=");
				if (splitindex == -1)
					continue;

				ret.Add(a.Substring(0, splitindex), a.Substring(splitindex + 2));
			}

			return ret;
		}
	}
}
