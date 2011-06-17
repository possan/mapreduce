using System;

namespace Possan.MapReduce
{
	public class BaseRecordDumper : IRecordDumper
	{
		protected virtual void WriteLine(string line) { }
		protected virtual void Close() { }

		public void Dump(IStorage storage, string[] batch, string title)
		{
			DumpHeader(title);
			foreach (var b in batch)
				Dump(storage, b, "");
			DumpFooter(title);
		}

		private void DumpFooter(string title)
		{
			if (string.IsNullOrEmpty(title))
				return;
			
			Console.WriteLine();
			Close();
		}

		public void Dump(IStorage storage, string batch, string title)
		{
			var rr = new RecordReader(storage, batch);
			Dump(rr, title);
		}

		public void Dump(IRecordReader<string, string> reader, string title)
		{
			DumpHeader(title);
			foreach (var k in reader.GetKeys())
				foreach (var v in reader.GetValues(k))
					WriteLine(k + " => " + v);
			DumpFooter(title);
		}

		private void DumpHeader(string title)
		{
			if (!string.IsNullOrEmpty(title))
			{
				WriteLine("------------------------------------------------------------");
				WriteLine(title);
				WriteLine("------------------------------------------------------------");
			}
		}
	}
}