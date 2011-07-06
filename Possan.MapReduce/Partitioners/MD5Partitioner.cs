using System.Security.Cryptography;
using System.Text;

namespace Possan.MapReduce.Partitioners
{
	public class MD5Partitioner : IPartitioner, IShardingPartitioner
	{
		public string Partition(string input)
		{
			if (input.Length == 0)
				return "";

			var md5 = MD5.Create();
			var dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(input));
			var sb = new StringBuilder();
			for (int i = 0; i < dataMd5.Length; i++)
				sb.AppendFormat("{0:x2}", dataMd5[i]);
			return sb.ToString();
		}

		public int Partition(string input, int numshards)
		{
			var md5 = MD5.Create();
			var dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(input));
		 	var sum = 0;
			for (int i = 0; i < dataMd5.Length; i++)
				sum += dataMd5[i];
			return sum % numshards;
		}
	}
}