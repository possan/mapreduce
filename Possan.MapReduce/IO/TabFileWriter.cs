using System;
using System.IO;
using System.Text;

namespace Possan.MapReduce.IO
{
	public class TabFileWriter : IRecordWriter<string, string>, IDisposable
	{
		readonly string _path;
		StringBuilder _buffer;
		int _buffered;

		public TabFileWriter(string path)
		{
			_path = path;
			_buffer = new StringBuilder();
			_buffered = 0;

		}

		public void Flush(bool force)
		{
			if (_buffered < 10 && !force)
				return;

			lock (_buffer)
			{
				try
				{
					if (!File.Exists(_path))
						File.WriteAllText(_path, "");
					File.AppendAllText(_path, _buffer.ToString());
					_buffer = new StringBuilder();
					_buffered = 0;
				}
				catch (Exception z)
				{
					Console.WriteLine(z);
				}
			}
		}

		public void Write(string key, string value)
		{
			lock (_buffer)
			{
				_buffer.Append(key);
				_buffer.Append("\t");
				_buffer.Append(value);
				_buffer.Append("\n");
				_buffered++;
			}

			Flush(false);
		}

		public void Close()
		{
			Flush(true);
		}

		public void Write(IRecordReader<string, string> reader)
		{
			foreach (var k in reader.GetKeys())
				foreach (var v in reader.GetValues(k))
					Write(k, v);
		}

		public void Write(IRecordStreamReader<string, string> reader)
		{
			string k, v;
			while (reader.Read(out k, out v))
				Write(k, v);
		}

		public void Dispose()
		{
			Close();
		}
	}
}
