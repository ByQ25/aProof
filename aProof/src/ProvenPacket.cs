using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace aProof
{
	public struct ProvenPacket
	{
		public string DictionaryHashId { get; }
		public HashSet<string> Assumptions { get; }
		public string Goal { get; }
		public string ProofInfo { get; }

		public ProvenPacket(
			string dictionaryHashId,
			HashSet<string> assumptions,
			string goal,
			string proofInfo
		)
		{
			this.DictionaryHashId = dictionaryHashId;
			this.Assumptions = assumptions;
			this.Goal = goal;
			this.ProofInfo = proofInfo;
		}

		private string DumpBaicDataToString()
		{
			StringBuilder output = new StringBuilder(8192);
			output.AppendLine(this.DictionaryHashId);
			if (this.Assumptions != null && this.Assumptions.Count > 0)
			{
				List<string> listedAssum = this.Assumptions.ToList();
				listedAssum.Sort();
				foreach (string assumption in listedAssum)
					output.AppendLine(assumption);
			}
			output.AppendLine(this.Goal);
			return output.ToString();
		}

		public override string ToString()
		{
			StringBuilder output = new StringBuilder(16384);
			output.AppendLine("Assumptions:");
			foreach (string assumption in Assumptions)
				output.AppendLine(assumption);
			output.AppendFormat("\nGoal: {0}\n\n", Goal);
			output.AppendLine("---- PROVER OUTPUT ----");
			output.AppendLine(ProofInfo);
			output.Append("\n\n--------------------------------\n");
			return output.ToString();
		}

		public override bool Equals(object obj)
		{
			// ProofInfo is not considered important in the context of object differentiation
			if (obj == null || !(obj is ProvenPacket))
				return false;
			ProvenPacket other = (ProvenPacket)obj;
			bool assumEquality = true;
			if (this.Assumptions != other.Assumptions
				&& this.Assumptions.Count == other.Assumptions.Count)
				foreach (string assumption in other.Assumptions)
					if (!this.Assumptions.Contains(assumption))
						assumEquality = false;
			return
				this.DictionaryHashId == other.DictionaryHashId
				&& this.Goal == other.Goal
				&& assumEquality;
		}

		public override int GetHashCode()
		{
			return this.DumpBaicDataToString().GetHashCode();
		}
	}
}