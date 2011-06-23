using System.IO;
using System.Text;

namespace Possan.MapReduce.IO
{
	public class TextFileRecordDumper : BaseRecordDumper
	{
		private readonly string _filename;
		private StringBuilder _builder;
		private int _index;

		public TextFileRecordDumper(string filename)
		{
			_filename = filename;
			_builder = new StringBuilder();
			_index = 0;
			File.WriteAllText(_filename,"");
		}

		void Flush(bool force)
		{
			if (_index < 50 && !force)
				return;
			_index = 0;
			File.AppendAllText(_filename, _builder.ToString());
			_builder = new StringBuilder();
		}

		protected override void WriteLine(string line)
		{
			_builder.Append(line).Append("\r\n");
			Flush(false);
		}

		protected override void Close()
		{
			Flush(true);
		}
	}
}