using System;
using System.IO;

namespace Possan.MapReduce.IO
{
	public class TabFileStreamReader : IRecordStreamReader<string, string>, IDisposable
	{
		private readonly string _path;
		private StreamReader _reader;
		private bool _closed;

		public TabFileStreamReader(string path)
		{
			_path = path;
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

			int idx = tmp.IndexOf('\t');
			if (idx == -1)
			{
				_closed = true;
				_reader.Close();
				return false;
			}
			
			key = tmp.Substring(0, idx);
			value = tmp.Substring(idx + 1);
			return true;
		}

		public void Dispose()
		{
			if( _reader != null )
				_reader.Close();
		}
	}
}