using System.Globalization;
using System;
using System.Text;
using Possan.MapReduce;

namespace WordCount
{
	class TestMapper : IMapper
	{
		public void Map(string key, string value, IMapperCollector collector)
		{
			// key = dummy
			// value = full file path
			// Console.WriteLine("Mapper started: key="+key+", value="+value);
			var words = value.Trim().Split(new[] { ' ', ',', ';', '.', '\n', '\r', '\t', '\"', '(', ')', '/', '\\', '{', '}', '[', ']', ':',  '<','>','-', '+', '!', '#', '$', '!', '?' });
			for (int i = 0; i < words.Length; i++)
			{
		//		if (i % 5 == 0 && i > 0)
		//		Console.WriteLine("mapper for " + key + ": " + i + " of " + words.Length + " done.");
				var tag = words[i];
				var tag2 = NormalizeString(tag).ToLower().Trim();
				if (tag2.Length > 2 && tag2.Length < 40)
					if (tag2 != "")
						collector.Collect(tag2, "1");
			}
		//	Console.WriteLine("Mapper done.");
		}

		string NormalizeString(string input)
		{
			if (input == null)
				return "";
			var sb = new StringBuilder();
			var normalizedString = input.Normalize(NormalizationForm.FormD);
			foreach(char c in normalizedString)
			{
				var uc = CharUnicodeInfo.GetUnicodeCategory(c);
				if (uc == UnicodeCategory.UppercaseLetter || uc == UnicodeCategory.LowercaseLetter || uc == UnicodeCategory.DecimalDigitNumber)
					sb.Append(c);
			}
			return sb.ToString();
		}
	}
}
