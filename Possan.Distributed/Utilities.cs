using System;
using System.Collections.Generic;
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


		public static void FillArgsFromJson(IJobArgs args, object jsonvalue)
		{
			if( jsonvalue == null )
				return;

			var strings = jsonvalue as string[];
			if( strings == null )
				return;
			
			foreach(var s in strings)
			{
				var i = s.IndexOf(":=");
				if( i == -1 )
					continue;

				args.Add(s.Substring(0,i), s.Substring(i+2));
			}
		}

		public static string BuildJsonFromArgs(IJobArgs args)
		{ 
			if (args == null)
				return "";

			var keys = new List<string>(args.GetKeys());
			if (keys.Count == 0)
				return "";

			var sb = new StringBuilder();
			sb.Append("[");
			bool firstkey = true;
			foreach (var k in keys)
			{
				foreach (var v in args.GetValues(k))
				{
					if (!firstkey)
						sb.Append(",");
					sb.Append("\"");
					sb.Append(EscapeJson(k + ":=" + v));
					sb.Append("\"");
					firstkey = false;
				}
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
}
