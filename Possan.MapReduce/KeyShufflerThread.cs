namespace Possan.MapReduce
{
	class KeyShufflerThread : PooledThread
	{
		public string Key;
		public IRecordReader<string, string> Reader;
		public IFileDestination<string, string> OutputFolder;

		public override void InnerRun()
		{
			var writer = OutputFolder.CreateWriter();
			var values = Reader.GetValues(Key);
			foreach (var value in values)
				writer.Write(Key, value);
		}
	}
}