using System;
using System.IO;

namespace Possan.MapReduce.IO
{
	public class TextFileLinesStreamReader : IRecordStreamReader<string, string>, IDisposable
	{
		private readonly string _path;
		private readonly string _key;
		private StreamReader _reader;
		private bool _closed;

		public TextFileLinesStreamReader(string path, string key)
		{
			_path = path;
			_key = key;
			_closed = false;
			_reader = null;
		}

		public bool Read(out string key, out string value)
		{
			key = "";
			value = "";

			if (_closed)
				return false;

			if (_reader == null)
			{
				try
				{
					_reader = new StreamReader(_path);
				}
				catch (Exception)
				{

				}
			}

			if (_reader == null)
			{
				_closed = true;
				return false;
			}

			string tmp = _reader.ReadLine();
			if (tmp == null)
			{
				_closed = true;
				_reader.Close();
				return false;
			}

			key = _key;
			value = tmp;

			return true;
		}

		public void Dispose()
		{
			if (_reader != null)
				_reader.Close();
		}
	}
}