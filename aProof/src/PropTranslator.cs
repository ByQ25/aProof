using System;
using System.Globalization;
using System.Collections.Generic;

namespace aProof.src
{
	static class PropTranslator
	{
		private static readonly string defaultLanguage;
		private static readonly Dictionary<string, string> enProps, plProps;
		
		static PropTranslator()
		{
			defaultLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			enProps = LoadDictionary(Properties.Resources.en_EN);
			plProps = LoadDictionary(Properties.Resources.pl_PL);
		}

		private static Dictionary<string, string> LoadDictionary(string dictinaryData)
		{
			Dictionary<string, string> output = new Dictionary<string, string>();
			if (dictinaryData != null)
			{
				string[] row;
				foreach (string line in dictinaryData.Split('\n'))
				{
					try
					{
						row = line.Split('=');
						if (row.Length == 2 && !output.ContainsKey(row[0]))
							output.Add(row[0], row[1]);
					}
					catch (Exception e)
					{
						Console.Error.Write(string.Format("{0}\n{1}", e.Message, e.StackTrace));
						continue;
					}
				}
			}
			return output;
		}

		public static string TranslateProp(string prop, string chosenLanguage)
		{
			string output;
			Dictionary<string, string> workingDict;
			switch (chosenLanguage)
			{
				case "en":
					workingDict = enProps;
					break;
				case "pl":
					workingDict = plProps;
					break;
				default:
					Console.Error.WriteLine(string.Format("WARNING: Translation to chosen language ({0}) is not supported.", chosenLanguage.ToUpper()));
					workingDict = new Dictionary<string, string>();
					break;
			}
			if (!workingDict.TryGetValue(prop, out output))
				output = prop;
			return output.Replace(":newli:", "\n");
		}

		public static string TranslateProp(string prop)
		{
			return TranslateProp(prop, defaultLanguage);
		}
	}
}
