namespace Possan.MapReduce.Partitioners
{
	public class StandardPartitioner : IPartitioner
	{
		private readonly int _shards;

		public StandardPartitioner(int shards)
		{
			_shards = shards;
		}

		public string Partition(string key)
		{
			int dummy = 0;
			foreach (var c in key)
				dummy += (int)c;
			return (dummy % _shards).ToString();
		}
	}
}