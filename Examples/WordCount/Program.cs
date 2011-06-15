using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Possan.MapReduce;
using Possan.MapReduce.Impl;

namespace mrtest
{
	class Program
	{
		static void Main(string[] args)
		{
			var __starttime = DateTime.Now;

			ThreadPool.MaximumTotalConcurrentThreads = 60;

			// var storage = new DiskStorage("c:\\temp\\MR");
			// var storage = new MySqlStorage("Datasource=localhost; Database=X; uid=X;pwd=X;Pooling=False;Max Pool Size=10;Connect Timeout=20;Connection Lifetime=50;", "");
			var storage = new GenericKeyValueStorage(new MemoryKeyValueStore());

			string prefix = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10) + "-";

			var files = Directory.GetFiles("c:\\temp\\smalltextfiles");
			foreach (var f in files)
				storage.Put(prefix + "mapper-input", Path.GetFileName(f), f);

			Console.WriteLine();
			Console.WriteLine("----------------------------------------------------------");
			Console.WriteLine();

			var reducerresults = MapperAndReducer.MapAndReduce(storage, new[] { prefix + "mapper-input" }, new TestMapper(), new TestReducer());

			Console.WriteLine();
			Console.WriteLine("----------------------------------------------------------");
			Console.WriteLine();

			// save reducer output to output folder
			var toplist = new List<ToplistItem>();
			foreach (var reduceresult in reducerresults)
			{
				var r = new RecordReader(storage, reduceresult);
				foreach (var k in r.GetKeys())
				{
					var v = r.GetValues(k);
					toplist.Add(new ToplistItem { Word = k, Hits = int.Parse(v.First()) });
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

			Console.WriteLine();
			Console.WriteLine("----------------------------------------------------------");
			Console.WriteLine();

			var __endtime = DateTime.Now;
			var dur_sec = __endtime.Subtract(__starttime).TotalSeconds;

			Console.WriteLine("Total execution time: " + dur_sec + " seconds.");

			/*
			storage.DeleteBatch(prefix + "mapper-input");
			foreach (var mapresult in mapresults)
				storage.DeleteBatch(mapresult);
			foreach (var shuffledfolder in shuffledfolders)
				storage.DeleteBatch(shuffledfolder);
			foreach (var reduceresult in reducerresults)
				storage.DeleteBatch(reduceresult);
			*/
		}

		class ToplistItem
		{
			public string Word;
			public int Hits;
		}
	}
}
