using System;
using System.IO;
using System.Text;

namespace Possan.Distributed
{
	internal class Utilities
	{
		public static string CombineURL(string start, string end)
		{
			var url = start;
			if (url.EndsWith("/"))
				url = url.Substring(0, url.Length - 1);
			url += end;
			return url;
		}


		public static byte[] ReadAllBytes(Stream input)
		{
			byte[] buffer = new byte[4 * 1024 * 1024];
			var ms = new MemoryStream();
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				ms.Write(buffer, 0, read);
			}
			return ms.ToArray();

		}

		public static string EscapeJson(string input, bool allowquotes)
		{
			var sbOut = new StringBuilder(input.Length);
			foreach (char ch in input)
			{
				if (!allowquotes && ch == '\"')
				{
					int ich = (int)ch;
					sbOut.Append(@"\u" + ich.ToString("x4"));
				}
				else if (Char.IsControl(ch) || ch == '\'')
				{
					int ich = (int)ch;
					sbOut.Append(@"\u" + ich.ToString("x4"));
				}
				else if (ch == '\"' || ch == '\\' || ch == '/' || ch == '\n' || ch == '\t' || ch == '\r' || ch == '\'')
				{
					sbOut.Append('\\');
					sbOut.Append(ch);
				}
				else
				{
					sbOut.Append(ch);
				}
			}
			return sbOut.ToString();
		}


		public static string EscapeJson(string input)
		{
			return EscapeJson(input, true);
		}
	}
}
