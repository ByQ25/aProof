using System;
using System.Collections.Generic;
using System.Text;

namespace aProof.src
{
	class MessageFormatter
	{
		private readonly Random rng;

		public MessageFormatter()
		{
			this.rng = new Random();
		}

		public MessageFormatter(int rngSeed)
		{
			this.rng = new Random(rngSeed);
		}

		private string TranslateExprIntoReadableText(string expr)
		{
			StringBuilder output = new StringBuilder(2048);
			output.Append(expr);
			// TODO: Implement TranslateExprIntoReadableText
			return output.ToString();
		}

		private string TranslateProofIntoReadableText(string proof)
		{
			StringBuilder output = new StringBuilder(4096);
			output.Append(proof);
			// TODO: Implement TranslateProofIntoReadableText
			return output.ToString();
		}

		private string MentionFactsSimilarity()
		{
			string output = "";
			switch (rng.Next(5))
			{
				case 0: output = "I know something similar!"; break;
				case 1: output = "Interesting, but did you know that one?"; break;
				case 2: output = "Ok, in this case, I think you shold know that..."; break;
				case 3: output = "But what about..."; break;
				case 4: output = "Hm, I think I know something interesting to you."; break;
				default: output = "Well..."; break;
			}
			return output;
		}

		private string MentionGoal(string inputGoal)
		{
			StringBuilder output = new StringBuilder(1024);
			switch (rng.Next(5))
			{
				// TODO: Come up with SMS about a goal
				case 0: output.Append("1"); break;
				case 1: output.Append("2"); break;
				case 2: output.Append("3"); break;
				case 3: output.Append("4"); break;
				case 4: output.Append("5"); break;
				default: output.Append("6"); break;
			}
			output.Append(TranslateExprIntoReadableText(inputGoal));
			return output.ToString();
		}

		private string MentionAssumptions(HashSet<string> inputAssumptions)
		{
			StringBuilder output = new StringBuilder(4096);
			switch (rng.Next(5))
			{
				// TODO: Come up with SMS about assumptions
				case 0: output.AppendLine("1"); break;
				case 1: output.AppendLine("2"); break;
				case 2: output.AppendLine("3"); break;
				case 3: output.AppendLine("4"); break;
				case 4: output.AppendLine("5"); break;
				default: output.AppendLine("6"); break;
			}
			foreach (string assumption in inputAssumptions)
				output.AppendLine(TranslateExprIntoReadableText(assumption));
			return output.ToString();
		}

		private string MentionProof(string proof)
		{
			StringBuilder output = new StringBuilder(4096);
			switch (rng.Next(5))
			{
				// TODO: Come up with SMS about proof
				case 0: output.AppendLine("1"); break;
				case 1: output.AppendLine("2"); break;
				case 2: output.AppendLine("3"); break;
				case 3: output.AppendLine("4"); break;
				case 4: output.AppendLine("5"); break;
				default: output.AppendLine("6"); break;
			}
			output.Append(TranslateProofIntoReadableText(proof));
			return output.ToString();
		}

		public string PrepareMessage()
		{
			string output = "";
			switch (rng.Next(10))
			{
				case 0: output = "Hello!"; break;
				case 1: output = "Hi."; break;
				case 2: output = "Welcome!"; break;
				case 3: output = "What's up?"; break;
				case 4: output = "Hi there..."; break;
				case 5: output = "Good morning,"; break;
				case 6: output = "How are you?"; break;
				case 7: output = "Nice to meet you!"; break;
				case 8: output = "Hello there!"; break;
				case 9: output = "What are we up to?"; break;
				default: output = "Well..."; break;
			}
			return output;
		}

		public string PrepareMessage(ProvenPacket inputFact, bool areFactsSimilar)
		{
			StringBuilder output = new StringBuilder(8192);
			if (areFactsSimilar)
				output.AppendLine(MentionFactsSimilarity());
			output.AppendLine(MentionGoal(inputFact.Goal));
			output.AppendLine(MentionAssumptions(inputFact.Assumptions));
			output.Append(MentionProof(inputFact.ProofInfo));
			return output.ToString();
		}
	}
}
