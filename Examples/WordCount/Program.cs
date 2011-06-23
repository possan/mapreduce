using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArticleKeywords.IO;
using Possan.MapReduce;
using Possan.MapReduce.IO;
using Possan.MapReduce.Partitioners;
using Possan.MapReduce.Util;

namespace mrtest
{
	class Program
	{
		static void Main(string[] args)
		{
			using (new Timing("Entire program"))
			{
				// ThreadPool.MaximumTotalConcurrentThreads = 60;

				Console.WriteLine();
				Console.WriteLine("----------------------------------------------------------");
				Console.WriteLine();

				string prefix = "wordcount-" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5) + "-";
				var tempoutput = "c:\\temp\\" + prefix + "-result.txt";// Path.GetTempPath() + "\\temp2";

				Fluently.Map.Input(new TextFilesFolderSource("c:\\temp\\textfiles"))
				//	.PartitionInputInMemoryUsing(new StandardPartitioner(9))
					.WithInMemoryMapper(new TestMapper())
					.ShuffleInMemoryUsing(new MD5Partitioner())
					.ReduceInMemoryUsing(new TestReducer())
					.CombineTo(new TabFileFolderWriter(tempoutput));

				Console.WriteLine();
				Console.WriteLine("----------------------------------------------------------");
				Console.WriteLine();

				ShowResults(tempoutput);

				Console.WriteLine();
				Console.WriteLine("----------------------------------------------------------");
				Console.WriteLine();
			}
		}

		static void ShowResults(string path)
		{
			var toplist = new List<ToplistItem>();

			var fs = new TabFileFolderSource(path);
			string fid;
			while (fs.ReadNext(out fid))
			{
				var f = fs.CreateStreamReader(fid);
				string k, v;
				while (f.Read(out k, out v))
				{
					var hits = int.Parse(v);
					toplist.Add(new ToplistItem { Word = k, Hits = hits });
				}
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
		}

		class ToplistItem
		{
			public string Word;
			public int Hits;
		}
	}
}
