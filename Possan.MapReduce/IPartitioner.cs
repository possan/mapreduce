namespace Possan.MapReduce
{
	public interface IPartitioner
	{
		string Partition(string input);
	}

	public interface IShardingPartitioner 
	{
		int Partition(string input, int numshards);
	}
}