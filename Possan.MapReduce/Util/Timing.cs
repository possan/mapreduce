using System;

namespace Possan.MapReduce.Util
{
	public class Timing : IDisposable
	{
		private readonly string _label;
		private DateTime _start;
		 
		public Timing(string label)
		{
			_label = label;
			Begin();
		}

		void Begin()
		{
			_start = DateTime.Now;
			// Console.WriteLine(_label + " started at: " + _start + "." + _start.Millisecond);
		}

		public void End()
		{
			var end = DateTime.Now;
		//	Console.WriteLine(_label + );
		//	Console.WriteLine(_label + " ended at: " + end + "." + end.Millisecond + " (started at: " + _start + "." + _start.Millisecond+")");
			var dur = end.Subtract(_start).TotalSeconds;
		//	if( dur < 1.0 )
		//		return;
			if (string.IsNullOrEmpty(_label))
				Console.WriteLine("Execution time: " + dur + " seconds.");
			else
				Console.WriteLine(_label + " time: " + dur + " seconds.");
		}

		public void Dispose()
		{
			End();
		}
	}
}
