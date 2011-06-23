using System;

namespace Possan.MapReduce.IO
{
	public class RecordWriterMapperCollector : IMapperCollector, IDisposable
	{
		private readonly IRecordWriter<string, string> _output;
		
		public RecordWriterMapperCollector( IRecordWriter<string, string> output )
		{
			_output = output;
		}

		public void Collect(string key, string value)
		{
			_output.Write(key,value);
		}

		public void Dispose()
		{
		}
	}
}