using System;

namespace Possan.MapReduce.IO
{
	public class FileDestinationMapperCollector : IMapperCollector, IDisposable
	{
		public IFileDestination<string, string> Output;
		private MemoryKeyValueReaderWriter _buffer;
		private int _counter;
		private int _total;

		public FileDestinationMapperCollector()
		{
			// _writer = output.CreateWriter();// writer;
			_counter = 0;
			_total = 0;
			_buffer = null;
		}

		public void Collect(string key, string value)
		{
			lock (this)
			{
				if (_buffer == null)
					_buffer = new MemoryKeyValueReaderWriter();

				_buffer.Write(key, value);
				_counter++;
				_total ++;
				if (_counter >= 1000)
				{
				// 	Console.WriteLine("File collector collected " + _total + " items");
					var writer = Output.CreateWriter();
					writer.Write(_buffer);
					writer.Dispose();
					_buffer = null;
					_counter = 0;
				}
			}
		}

		public void Dispose()
		{
			lock (this)
			{
				if( _total > 500 )
					Console.WriteLine("File collector collected "+_total+" items");
				
				if (_buffer != null && _counter > 0)
				{
					var writer = Output.CreateWriter();
					writer.Write(_buffer);
					writer.Dispose();
					_buffer = null;
				}
			}
		}
	}
}