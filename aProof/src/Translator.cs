using System.Text;

namespace aProof
{
	class Translator : ITranslator
	{
		private string TranslateIntoEnglish(string input)
		{
			// TODO: Real implementation for en-EN pending
			StringBuilder output = new StringBuilder(input.Length * 2);
			output.AppendLine("en-EN");
			return output.ToString();
		}

		private string TranslateIntoPolish(string input)
		{
			// TODO: Real implementation for pl-PL  pending
			StringBuilder output = new StringBuilder(input.Length * 2);
			output.AppendLine("pl-PL");
			return output.ToString();
		}

		public string Translate(string input, string chosenLanguage)
		{
			string output;
			switch (chosenLanguage)
			{
				case "en-EN": output = TranslateIntoEnglish(input); break;
				case "pl-PL": output = TranslateIntoPolish(input); break;
				default: output = string.Format("-------- WARNING!: translation to user's system language ( {0} ) is not supported. --------\n\n{1}", chosenLanguage, input); break;
			}
			return output;
		}
	}
}
