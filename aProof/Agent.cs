using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace aProof
{
	class Agent
	{
		private enum ExpressionType { Assumptions, Goals }
		private enum ExpressionComplexity { Simple, Relational, GeneralQuantifiers }
		private readonly bool isDebugModeOn;
		private readonly string debugLogFilePath;
		private Random rng;
		private HashSet<string> assumptions, goals;
		private readonly DictHandler dictionary;
		private readonly ProverHelper prover;
		private readonly HashSet<ProvenPacket> facts;	// Facts = proven goals, initially empty because facts must be proven
		public HashSet<ProvenPacket> Facts { get { return new HashSet<ProvenPacket>(facts); } }

		public Agent(DictHandler dictionary, HashSet<string> assumptions, HashSet<string> goals)
		{
			this.isDebugModeOn = SimulationSettings.Default.IS_IN_DEBUG_MODE;
			this.debugLogFilePath = @"debug.log";
			this.dictionary = dictionary;
			this.assumptions = assumptions;
			this.goals = goals;
			this.rng = new Random();
			this.prover = new ProverHelper();
			this.facts = new HashSet<ProvenPacket>();
		}

		public Agent(DictHandler dictionary) : this(dictionary, new HashSet<string>(), new HashSet<string>())
		{
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Assumptions);
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Goals);
		}

		private string ReturnSign()
		{
			return rng.Next(100) < 30 ? "-" : "";
		}

		private string ReturnQuantifierWithSpace()
		{
			return rng.Next(2) == 1 ? "all " : "exists ";
		}

		private string ReturnOperator(int option)
		{
			switch (option)
			{
				case 0: return " | ";
				case 1: return " & ";
				case 2: return " -> ";
				case 3: return " <-> ";
				default: return "";
			}
		}

		private string GenerateSimpleSubExpr()
		{
			if (dictionary.Vars.Length == 0 && dictionary.Nouns.Length == 0)
				return "";
			if (dictionary.Vars.Length == 0 && dictionary.Nouns.Length > 0)
				return dictionary.Nouns[rng.Next(dictionary.Nouns.Length)];
			if (dictionary.Vars.Length > 0 && dictionary.Nouns.Length == 0)
				return dictionary.Vars[rng.Next(dictionary.Vars.Length)];
			if (rng.Next(2) == 0)
				return dictionary.Vars[rng.Next(dictionary.Vars.Length)];
			else
				return ReturnSign() + dictionary.Nouns[rng.Next(dictionary.Nouns.Length)];
		}

		private string GenerateRelationalSubExpr()
		{
			StringBuilder output = new StringBuilder(256);
			if (dictionary.Relations.Length > 0 && (dictionary.Vars.Length > 0 || dictionary.Nouns.Length > 0))
			{
				KeyValuePair<string, uint> relation = dictionary.RelationsWithSizes.ElementAt(rng.Next(dictionary.RelationsWithSizes.Count));
				output.Append(ReturnSign());
				output.Append(relation.Key);
				output.Append("(");
				for (int i = 0; i < relation.Value; ++i)
				{
					output.Append(GenerateSimpleSubExpr());
					output.Append(i > relation.Value - 2 ? ")" : ", ");
				}
			}
			else output.Append(GenerateSimpleSubExpr());
			return output.ToString();
		}

		private string GenerateGeneralQuantifiersExpr()
		{
			string relationalSubExpr = GenerateRelationalSubExpr();
			if (dictionary.Relations.Length > 0 && dictionary.Vars.Length > 0)
			{
				List<string> varsPool = new List<string>();
				HashSet<string> usedVars = new HashSet<string>();
				Regex regex = new Regex(@"(\w+)");
				MatchCollection matches = regex.Matches(relationalSubExpr);
				StringBuilder output = new StringBuilder(256);
				varsPool.AddRange(dictionary.Vars);
				foreach (Match match in matches)
					if (varsPool.Contains(match.Value))
						usedVars.Add(match.Value);
				foreach (string var in usedVars)
				{
					output.Append(ReturnQuantifierWithSpace());
					output.Append(var);
					output.Append(" ");
				}
				output.Append(relationalSubExpr);
				return output.ToString();
			}
			return relationalSubExpr;
		}

		private string GeneratePartialExpr(ExpressionComplexity exprComplexity)
		{
			switch (exprComplexity)
			{
				case ExpressionComplexity.Simple: return GenerateSimpleSubExpr();
				case ExpressionComplexity.Relational: return GenerateRelationalSubExpr();
				case ExpressionComplexity.GeneralQuantifiers: return GenerateGeneralQuantifiersExpr();
				default: return "";
			}
		}

		private string GenerateExpr(int wordsCount, ExpressionComplexity exprComplexity)
		{
			StringBuilder output = new StringBuilder(512);
			for (int i = 0; i < wordsCount; ++i)
			{
				output.Append(GeneratePartialExpr(exprComplexity));
				if (i < wordsCount - 1) output.Append(ReturnOperator(rng.Next(4)));
			}
			output.Append(".");
			return output.ToString();
		}

		private void AddAssumptionOrGoal(string expr, ExpressionType exprType)
		{
			switch (exprType)
			{
				case ExpressionType.Assumptions: this.assumptions.Add(expr); break;
				case ExpressionType.Goals: this.goals.Add(expr); break;
			}
		}

		private void DrawInitialAssumptionsOrGoals(DictHandler dictionary, ExpressionType exprType)
		{
			int maxExprCount, exprCount, maxWordsCount, wordsCount;

			switch (exprType)
			{
				case ExpressionType.Assumptions: maxExprCount = (int)SimulationSettings.Default.MAX_ASSUMPTIONS_DURING_INIT; break;
				case ExpressionType.Goals: maxExprCount = (int)SimulationSettings.Default.MAX_GOALS_DURING_INIT; break;
				default: maxExprCount = 1; break;
			}
			exprCount = rng.Next(maxExprCount + 1);
			switch (exprType)
			{
				case ExpressionType.Assumptions: maxWordsCount = (int)SimulationSettings.Default.MAX_WORDS_COUNT_FOR_ASSUMPTION; break;
				case ExpressionType.Goals: maxWordsCount = (int)SimulationSettings.Default.MAX_WORDS_COUNT_FOR_GOAL; break;
				default: maxWordsCount = 1; break;
			}
			for (int i = 0; i < exprCount; ++i)
			{
				wordsCount = rng.Next(maxWordsCount + 1);
				switch (rng.Next(3))
				{
					case 0: AddAssumptionOrGoal(GenerateExpr(wordsCount, ExpressionComplexity.Simple), exprType); break;				// Simple expression
					case 1: AddAssumptionOrGoal(GenerateExpr(wordsCount, ExpressionComplexity.Relational), exprType); break;			// Relational expression
					case 2: AddAssumptionOrGoal(GenerateExpr(1, ExpressionComplexity.GeneralQuantifiers), exprType); break;				// General quantifiers: exists, all
				}
			}
			// Removing empty assumptions / goals created because of particular dictionary
			this.assumptions.Remove(".");
			this.goals.Remove(".");
		}

		private HashSet<string> DrawTemporaryAssumptionsOrGoals(ExpressionType exprType)
		{
			List<string> outputSet;
			switch (exprType)
			{
				case ExpressionType.Assumptions:
					outputSet = new List<string>(assumptions);
					int exprToExcludeCount = rng.Next(assumptions.Count);
					for (int i = 0; i < exprToExcludeCount; ++i)
						outputSet.RemoveAt(rng.Next(outputSet.Count));
					return new HashSet<string>(outputSet);
				case ExpressionType.Goals:
					outputSet = new List<string>(goals);
					return new HashSet<string>() { outputSet[rng.Next(outputSet.Count)] };
				default:
					return new HashSet<string>();
			}
		}

		public void VerifyGoals()
		{
			bool isProofFound;
			if (assumptions.Count > 0 && goals.Count > 0)
			{
				HashSet<string> currAssumptions = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
				foreach (string goal in goals)
				{
					currAssumptions = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
					for (int i = 0; i < SimulationSettings.Default.MAX_PROOF_SEARCH_ATTEMPTS; ++i)
					{
						isProofFound = prover.SearchForProof(currAssumptions, goal);
						if (isDebugModeOn)
							LogCurrentStateAsDebug(
								new ProvenPacket(
									dictionary.HashId,
									currAssumptions,
									goal,
									prover.GetPartialOutput()
								)
							);
						if (isProofFound)
						{
							this.Facts.Add(
								new ProvenPacket(
									dictionary.HashId,
									currAssumptions,
									goal,
									prover.GetPartialOutput()
								)
							);
							break;
						}
						else
							currAssumptions = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
					}
				}
			}
		}

		public void AddExternalKnownFact(ProvenPacket factToAdd)
		{
			foreach (string assumption in factToAdd.Assumptions)
				this.assumptions.Add(assumption);
			this.goals.Add(factToAdd.Goal);
			this.facts.Add(factToAdd);
		}

		private void LogCurrentStateAsDebug(ProvenPacket pp)
		{
			try
			{
				if (!File.Exists(this.debugLogFilePath))
					File.Create(this.debugLogFilePath);
				using (StreamWriter sw = new StreamWriter(this.debugLogFilePath, true))
					sw.Write(pp.ToString());
			}
			catch { return; }
		}

		public class AgentException : ApplicationException
		{
			public AgentException(string errMsg) : base(errMsg) { }
		}

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
				string proofInfo)
			{
				this.DictionaryHashId = dictionaryHashId;
				this.Assumptions = assumptions;
				this.Goal = goal;
				this.ProofInfo = proofInfo;
			}

			public string DumpBaicDataToString()
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
}
