namespace Possan.MapReduce.Partitioners
{
	public class FirstCharacterPartitioner : IPartitioner
	{
		public string Partition(string key)
		{
			if (key.Length == 0)
				return "";
			return key.ToLowerInvariant()[0].ToString();
		}
	}
}