namespace Possan.MapReduce.Partitioners
{
	public class FirstCharacterPartitioner : IPartitioner, IShardingPartitioner
	{
		public string Partition(string key)
		{
			if (key.Length == 0)
				return "";
			return key.ToLowerInvariant()[0].ToString();
		}
		
		public int Partition(string key, int numshards)
		{
			if (key.Length == 0)
				return 0;

			var c = (int)key.ToLowerInvariant()[0];
			c %= numshards;
			return c;
		}
	}
}