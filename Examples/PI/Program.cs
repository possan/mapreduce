using System;
using System.Globalization;
using Possan.MapReduce;
using Possan.MapReduce.Reducers;
using Possan.MapReduce.IO;
using Possan.MapReduce.Util;

namespace PI
{
	/// <summary>
	/// Estimate pi using this method: http://code.google.com/edu/parallel/mapreduce-tutorial.html 
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			var timer = new Timing("Entire app");
			
			int numpoints = 10000;
			
			var tempInput = new DummyFileSource(numpoints, 15, "key", "value");
			var tempOutput = new InMemoryTempFolder();

			Fluently.Map.Input(tempInput).WithInMemoryMapper(new RandomPointInCircleMapper()).ReduceInMemoryUsing(new SummaryReducer()).CombineTo(tempOutput);

			int num_inside = 0;
			string k, v;
			while (tempOutput.Buffer.Read(out k, out v))
			{
				// Console.WriteLine(k + " >>> " + v);
				num_inside += int.Parse(v, CultureInfo.InvariantCulture);
			}
			double pi = num_inside * 4.0 / numpoints;

			Console.WriteLine("PI estimated to: " + pi + " (" + num_inside + " of " + numpoints + " inside circle)");

			timer.End();
		}
	}

	class RandomPointInCircleMapper : IMapper
	{
		public void Map(string key, string value, IMapperCollector collector)
		{
			var r = new Random();
			var x = -1.0 + (r.NextDouble() * 2.0);
			var y = -1.0 + (r.NextDouble() * 2.0);
			var d = x * x + y * y;
			if (d >= 1.0)
				return;
			collector.Collect("x", "1");
		}
	}
}
