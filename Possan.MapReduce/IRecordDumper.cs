namespace Possan.MapReduce
{
	public interface IRecordDumper
	{
		void Dump(IStorage storage, string[] batchnames, string title);
		void Dump(IStorage storage, string batch, string title);
		void Dump(IRecordReader<string, string> reader, string title);
	}
}