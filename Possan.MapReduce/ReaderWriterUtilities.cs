using System;
using System.Collections.Generic;

namespace Possan.MapReduce
{
	public class ReaderWriterUtilities
	{
		public static void Copy(IRecordReader<string, string> reader, IRecordWriter<string, string> writer)
		{
			var keys = new List<string>(reader.GetKeys());
			for (int i = 0; i < keys.Count; i++)
			{
				var key = keys[i];
				if (i % 100 == 0 && i > 0)
					Console.WriteLine("Copy key " + i + "/" + keys.Count);

				var values = reader.GetValues(key);
				foreach (var value in values)
					writer.Write(key, value);
			}
		}
	}
}
