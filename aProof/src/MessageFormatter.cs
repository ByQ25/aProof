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
				case 0: output.AppendLine("The following expression is being true: "); break;
				case 1: output.AppendLine("I've found out about: "); break;
				case 2: output.AppendLine("From what I know, the truth is: "); break;
				case 3: output.AppendLine("This seems to be valid: "); break;
				case 4: output.AppendLine("What seems to be true is: "); break;
				default: output.AppendLine("Well... : "); break;
			}
			output.AppendLine(inputGoal);
			return output.ToString();
		}

		private string MentionAssumptions(HashSet<string> inputAssumptions)
		{
			StringBuilder output = new StringBuilder(4096);
			switch (rng.Next(5))
			{
				case 0: output.AppendLine("Using following assumptions:"); break;
				case 1: output.AppendLine("Assuming that:"); break;
				case 2: output.AppendLine("If we'd assume:"); break;
				case 3: output.AppendLine("My assumptions are:"); break;
				case 4: output.AppendLine("With assumptions listed below:"); break;
				default: output.AppendLine("Well... : "); break;
			}
			foreach (string assumption in inputAssumptions)
				output.AppendLine(assumption);
			return output.ToString();
		}

		private string MentionProof(string proof)
		{
			StringBuilder output = new StringBuilder(4096);
			switch (rng.Next(5))
			{
				case 0: output.AppendLine("Here's a proof:"); break;
				case 1: output.AppendLine("The proof is:"); break;
				case 2: output.AppendLine("Proof:"); break;
				case 3: output.AppendLine("That is how to prove it:"); break;
				case 4: output.AppendLine("About the proof I definitely owe you..."); break;
				default: output.AppendLine("Well... : "); break;
			}
			output.Append(proof);
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
