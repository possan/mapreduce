﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Possan.MapReduce.Partitioners
{
	public class SHA1Partitioner : IPartitioner
	{
		/// <summary>
		/// Returns a PHP-Style SHA1-Hash of input string
		/// </summary>
		/// <param name="input">Input string</param>
		/// <returns>SHA1 Hash</returns>		
		string SHA1(string input)
		{
			SHA1 md5 = System.Security.Cryptography.SHA1.Create();
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
			return SHA1(input);
		}

		public string Partition(string input, int numshards)
		{
			throw new NotImplementedException();
		}
	}
}