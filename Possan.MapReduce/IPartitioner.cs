namespace Possan.MapReduce
{
	public interface IPartitioner
	{
		string Partition(string input);
	}
}