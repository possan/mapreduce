﻿using System.Collections.Generic;

namespace Possan.Distributed.Sandbox
{
	public interface ISandboxedJobArgs
	{
		void Add(string key, string value);
		IEnumerable<string> GetKeys();
		string Get(string key);
		string Get(string key, string defaultvalue);
		IList<string> GetValues(string key);
	}
}