using System;

namespace Possan.MapReduce
{
	public class Timing : IDisposable
	{
		private readonly string _label;
		private static DateTime _start;

		public Timing()
		{
			_start = DateTime.Now;
		}

		public Timing(string label)
			: this()
		{
			_label = label;
		}

		public double End()
		{
			var end = DateTime.Now;
			var dur = end.Subtract(_start).TotalSeconds;
			if (string.IsNullOrEmpty(_label))
				Console.WriteLine("Execution time: " + dur + " seconds.");
			else
				Console.WriteLine(_label + " time: " + dur + " seconds.");
			return (double)dur;
		}

		public void Dispose()
		{
			End();
		}
	}
}
