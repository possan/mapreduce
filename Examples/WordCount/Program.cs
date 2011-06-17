using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Possan.MapReduce;

namespace mrtest
{
	class Program
	{
		static void Main(string[] args)
		{
			using (new Timing("Entire program"))
			{
				ThreadPool.MaximumTotalConcurrentThreads = 60;

				var input = new MemoryKeyValueReaderWriter();
				 
				var files = Directory.GetFiles("c:\\temp\\logfiles");
				foreach (var f in files)
					input.Write(Path.GetFileName(f), f);

				Console.WriteLine();
				Console.WriteLine("----------------------------------------------------------");
				Console.WriteLine();
				 
				var output = new MemoryKeyValueReaderWriter();
				MapperAndReducer.MapAndReduce(input, output, new TestMapper(), new TestReducer());

				Console.WriteLine();
				Console.WriteLine("----------------------------------------------------------");
				Console.WriteLine();
				 
				var toplist = new List<ToplistItem>();

				foreach (var k in output.GetKeys())
				{
					var v = output.GetValues(k);
					toplist.Add(new ToplistItem { Word = k, Hits = int.Parse(v.First()) });
				}

				toplist.Sort((a, b) =>
				{
					var x = -a.Hits.CompareTo(b.Hits);
					if (x != 0)
						return x;
					return a.Word.CompareTo(b.Word);
				});

				var sb = new StringBuilder();
				foreach (var k in toplist)
					sb.Append(string.Format("{0} {1}\n", k.Hits, k.Word));
				string report = sb.ToString();
				File.WriteAllText("c:\\temp\\reducer-output.txt", report);
				Console.WriteLine(report);

				Console.WriteLine();
				Console.WriteLine("----------------------------------------------------------");
				Console.WriteLine();
			} 
		}

		class ToplistItem
		{
			public string Word;
			public int Hits;
		}
	}
}
