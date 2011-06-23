namespace Possan.MapReduce.IO
{
	public class RecordWriterReducerCollector : IReducerCollector
	{
		private readonly IRecordWriter<string, string> _writer;

		public RecordWriterReducerCollector(IRecordWriter<string, string> writer)
		{
			_writer = writer;
		}

		public void Collect(string key, string value)
		{
			_writer.Write(key, value);
		}
	}
	 
}