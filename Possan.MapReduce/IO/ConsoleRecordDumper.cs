using System;

namespace Possan.MapReduce.IO
{
	public class ConsoleRecordDumper : BaseRecordDumper
	{
		protected override void WriteLine(string line)
		{
			Console.WriteLine(line);
		} 
	}
}
