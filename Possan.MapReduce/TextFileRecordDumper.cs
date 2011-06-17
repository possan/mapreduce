using System.IO;
using System.Text;

namespace Possan.MapReduce
{
	public class TextFileRecordDumper : BaseRecordDumper
	{
		private readonly string _filename;
		private StringBuilder _builder;

		public TextFileRecordDumper(string filename)
		{
			_filename = filename;
			_builder = new StringBuilder();
		}

		protected override void WriteLine(string line)
		{
			_builder.Append(line).Append("\r\n");
		}

		protected override void Close()
		{
			File.WriteAllText(_filename, _builder.ToString());
		}
	}
}