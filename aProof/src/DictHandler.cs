using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace aProof
{
	class DictHandler
	{
		private readonly string[][] dictionary;

		public readonly string HashId;
		public string[] Vars => dictionary[0];
		public string[] Nouns => dictionary[1];
		public string[] Relations => dictionary[2];
		public string[] RelationsSizes => dictionary[3];
		public Dictionary<string, uint> RelationsWithSizes { get; }

		public DictHandler(string dictionaryPath)
		{
			this.dictionary = LoadDictionariesFromCsv(dictionaryPath);
			this.RelationsWithSizes = CreateRelationsDictionary(this.Relations, this.RelationsSizes);
			this.HashId = CalculateSha512Hash(SerilizeToCsv());
		}

		private string[][] LoadDictionariesFromCsv(string path)
		{
			string[][] dictionary = new string[4][];

			if (File.Exists(path))
			{
				try { using (Stream stream = new FileStream(path, FileMode.Open)) { } }
				catch (IOException exc) { throw new DictHandlerException(exc.Message); }
				using (StreamReader sr = new StreamReader(path))
				{
					string input = sr.ReadToEnd();
					string[] inSplit = input.Split('\n');

					if (inSplit.Length > 3)
					{
						for (int i = 0; i < 4; ++i)
							dictionary[i] = inSplit[i].TrimEnd('\r', ';').Split(';');
						if (dictionary[2].Length != dictionary[3].Length)
							throw new DictHandlerException("Dictionary file is formatted incorrectly. Last two rows of data have to be the same size.");
						return dictionary;
					}
					else throw new DictHandlerException("Dictionary file is formatted incorrectly. The dictionary require at least 4 data rows.");
				}
			}
			else throw new DictHandlerException("No dictionary file.");
		}

		private uint[] ConvertSizesToUint(string[] sizesInStrings)
		{
			uint[] sizesInUints = new uint[sizesInStrings.Length];
			for (int i = 0; i < sizesInStrings.Length; ++i)
				if (uint.TryParse(sizesInStrings[i], out uint parsedUint) && parsedUint > 0) sizesInUints[i] = parsedUint;
				else throw new DictHandlerException("Dictionary file is formatted incorrectly. Last row sould consist of positive numbers only.");
			return sizesInUints;
		}

		private Dictionary<string, uint> CreateRelationsDictionary(string[] relations, string[] relationsSizes)
		{
			Dictionary<string, uint> relationsWithSizes = new Dictionary<string, uint>();
			uint[] tempSizes = new uint[relationsSizes.Length];
			tempSizes = ConvertSizesToUint(relationsSizes);
			for (int i = 0; i < relations.Length; ++i)
				if (!relationsWithSizes.ContainsKey(relations[i]))
					relationsWithSizes.Add(relations[i], tempSizes[i]);
			return relationsWithSizes;
		}

		private string SerilizeToCsv()
		{
			StringBuilder sb = new StringBuilder(4096);
			foreach (string[] list in dictionary)
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

		private string CalculateSha512Hash(string input)
		{
			StringBuilder output = new StringBuilder(128);
			using (SHA512 sha512 = SHA512.Create())
			{
				byte[] shaBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
				for (int i = 0; i < shaBytes.Length; ++i)
					output.Append(shaBytes[i].ToString("x2"));
			}
			return output.ToString();
		}

		public bool ValidateInputAgainstDictionary(string input)
		{
			Regex regex = new Regex(@"(\w+)");
			MatchCollection matches = regex.Matches(input);
			string[] additional = new string[] { "exists", "all", "assign"};

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
			string[] additional = new string[] { "exists", "all", "assign" };

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
