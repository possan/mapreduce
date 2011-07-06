namespace Possan.MapReduce.Partitioners
{
	public class StandardPartitioner : IPartitioner, IShardingPartitioner
	{
		private readonly int _shards;

		public StandardPartitioner(int shards)
		{
			_shards = shards;
		}

		public StandardPartitioner() : this(10)
		{
		}

		public string Partition(string key)
		{
			return Partition(key, _shards).ToString();
		}

		public int Partition(string key, int numshards)
		{
			int dummy = 0;
			foreach (var c in key)
				dummy += (int)c;
			return (dummy % numshards);
		}
	}
}