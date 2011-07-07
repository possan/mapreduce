using System;
using System.Collections.Generic;
using Possan.Distributed;

namespace Possan.MapReduce.Distributed
{
	class JobUtilities
	{
		public static IFileDestination<string, string> ParseAndCreateFileDestination(string input)
		{
			var aa = input.Split('=');
			return Activator.CreateInstance(Type.GetType(aa[0]), new object[] { aa[1] }) as IFileDestination<string, string>;
		}

		public static IFileSource<string, string> ParseAndCreateFileSource(string input)
		{
			var aa = input.Split('=');
			return Activator.CreateInstance(Type.GetType(aa[0]), new object[] { aa[1] }) as IFileSource<string, string>;
		}

		public static IList<IFileDestination<string, string>> ParseAndCreateFileDestinations(IEnumerable<string> outputs)
		{
			var ret = new List<IFileDestination<string, string>>();
			foreach (var output in outputs)
			{
				var writer = ParseAndCreateFileDestination(output);
				if (writer != null)
					ret.Add(writer);
			}
			return ret;
		}

		public static IList<IFileSource<string, string>> ParseAndCreateFileSources(IEnumerable<string> inputs)
		{
			var ret = new List<IFileSource<string, string>>();
			foreach (var input in inputs)
			{
				var reader = ParseAndCreateFileSource(input);
				if (reader != null)
					ret.Add(reader);
			}
			return ret;
		}
	}
}
