using System;

namespace Possan.MapReduce
{
	public class ConsoleRecordDumper : BaseRecordDumper
	{
		protected override void WriteLine(string line)
		{
			Console.WriteLine(line);
		} 
	}
}
