using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace aProof
{
	class DictHandler
	{
		private readonly List<string>[] dictionary;

		public string[] Vars => dictionary[0].ToArray();
		public string[] Nouns => dictionary[1].ToArray();
		public string[] Relations => dictionary[2].ToArray();

		public DictHandler(string[] vars, string[] nouns, string[] relations)
		{
			this.dictionary = new List<string>[3] {
				vars.ToList<string>(),
				nouns.ToList<string>(),
				relations.ToList<string>(),
			};
		}

		public DictHandler(string dictionaryPath)
		{
			try { this.dictionary = LoadDictionariesFromCsv(dictionaryPath); }
			catch (DictHandlerException) { this.dictionary = new List<string>[3] { new List<string>(), new List<string>(), new List<string>() }; }
		}

		private List<string>[] LoadDictionariesFromCsv(string path)
		{
			List<string>[] dictionary = new List<string>[3];

			if (File.Exists(path))
				using (StreamReader sr = new StreamReader(path))
				{
					string input = sr.ReadToEnd();
					string[] inSplit = input.Split('\n');
					if (inSplit.Length > 2)
					{
						for (int i = 0; i < 3; ++i)
							dictionary[i] = inSplit[i].TrimEnd('\r', ';').Split(';').ToList<string>();
						return dictionary;
					}
					else throw new DictHandlerException("Dictionary file is formatted incorrectly.");
				}
			else throw new DictHandlerException("No dictionary file.");
		}

		public string SerilizeToCsv()
		{
			StringBuilder sb = new StringBuilder(4096);
			foreach (List<string> list in dictionary)
			{
				foreach (string item in list)
				{
					sb.Append(item);
					sb.Append(';');
				}
				sb.Append('\n');
			}
			return sb.ToString();
		}

		// TODO: Is this method usueful?
		public bool ValidateInputAgainstDictionary(string input)
		{
			Regex regex = new Regex(@"(\w+)");
			MatchCollection matches = regex.Matches(input);
			List<string> additional = new List<string> { "exists", "all", "assign"};

			foreach (Match match in matches)
			{
				if
				(
					dictionary[0].Contains(match.Value)
					|| dictionary[1].Contains(match.Value)
					|| dictionary[2].Contains(match.Value)
					|| additional.Contains(match.Value)
				) return true;
			}

			return false;
		}

		public bool ValidateInputAgainstDictionary(HashSet<string> input)
		{
			HashSet<string> words = new HashSet<string>();
			Regex regex = new Regex(@"(\w+)");
			MatchCollection matches;
			List<string> additional = new List<string> { "exists", "all", "assign" };

			foreach (string item in input)
			{
				matches = regex.Matches(item);
				foreach (Match match in matches)
					words.Add(match.Value);
			}

			foreach (string word in words)
			{
				if
				(
					dictionary[0].Contains(word)
					|| dictionary[1].Contains(word)
					|| dictionary[2].Contains(word)
					|| additional.Contains(word)
				) continue;
				return false;
			}

			return true;
		}

		public class DictHandlerException : ApplicationException
		{
			public DictHandlerException(string errMsg) : base(errMsg) { }
		}
	}
}
