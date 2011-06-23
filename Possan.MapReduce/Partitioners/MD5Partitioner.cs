using System.Security.Cryptography;
using System.Text;

namespace Possan.MapReduce.Partitioners
{
	public class MD5Partitioner : IPartitioner
	{
		/// <summary>
		/// Returns PHP-Style MD5-Hash of input string
		/// </summary>
		/// <param name="input">Input String</param>
		/// <returns>MD5 Hash</returns>
		string MD5(string input)
		{
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(input));
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < dataMd5.Length; i++)
				sb.AppendFormat("{0:x2}", dataMd5[i]);
			return sb.ToString();
		}


		public string Partition(string input)
		{
			if (input.Length == 0)
				return "";
			return MD5(input);
		}
	}
}