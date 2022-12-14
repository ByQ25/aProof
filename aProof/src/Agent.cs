using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
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

		public Agent(DictHandler dictionary, HashSet<string> assumptions, HashSet<string> goals, int rngSeed)
		{
			this.isDebugModeOn = SimulationSettings.Default.IS_IN_DEBUG_MODE;
			this.debugLogFilePath = SimulationSettings.Default.DEBUG_FILE_PATH;
			this.dictionary = dictionary;
			this.assumptions = assumptions;
			this.goals = goals;
			this.rng = new Random(rngSeed);
			this.prover = new ProverHelper();
			this.facts = new HashSet<ProvenPacket>();
		}

		public Agent(DictHandler dictionary, int rngSeed) : this(dictionary, new HashSet<string>(), new HashSet<string>(), rngSeed)
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

		public void RefreshAssumptionsAndGoals()
		{
			HashSet<string> randomAssumptionsToReuse = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
			HashSet<string> randomGoalsToReuse = DrawTemporaryAssumptionsOrGoals(ExpressionType.Goals);
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Assumptions);
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Goals);
			this.assumptions.Concat(randomAssumptionsToReuse);
			this.goals.Concat(randomGoalsToReuse);
			foreach (ProvenPacket fact in this.facts)
			{
				this.assumptions.Concat(fact.Assumptions);
				this.goals.Add(fact.Goal);
			}
			// TODO: Assumptions and goals went over 2000 once...
		}

		private HashSet<string> DrawTemporaryAssumptionsOrGoals(ExpressionType exprType)
		{
			int exprToExcludeCount = 0;
			List<string> outputSet = new List<string>();
			switch (exprType)
			{
				case ExpressionType.Assumptions:
					if (assumptions.Count > 0)
					{
						outputSet = new List<string>(assumptions);
						exprToExcludeCount = rng.Next(assumptions.Count);
					}
					break;
				case ExpressionType.Goals:
					if (goals.Count > 0)
					{
						outputSet = new List<string>(goals);
						exprToExcludeCount = rng.Next(goals.Count);
					}
					break;
				default:
					break;
			}
			for (int i = 0; i < exprToExcludeCount; ++i)
				outputSet.RemoveAt(rng.Next(outputSet.Count));
			return new HashSet<string>(outputSet);
		}

		private bool IsGoalWellMatchedWithAssumptionsCheck(string goal, HashSet<string> assumptions)
		{
			Regex regex = new Regex(@"(\w+)");
			MatchCollection matches = regex.Matches(goal);
			HashSet<string> relationsUsedInGoal = new HashSet<string>();
			foreach (Match match in matches)
				if (this.dictionary.Relations.Contains(match.Value))
					relationsUsedInGoal.Add(match.Value);
			if (relationsUsedInGoal.Count == 0)
				return true;
			foreach (string relation in relationsUsedInGoal)
				foreach (string assumption in assumptions)
					if (assumption.Contains(relation))
						return true;
			return false;
		}

		public void VerifyGoals()
		{
			uint drawsCounter, maxDraws, maxProofSearchAttempts;
			bool isProofFound;
			ProvenPacket tmpPacket;
			HashSet<string> currAssumptions;
			maxDraws = SimulationSettings.Default.MAX_REPEATS_FOR_DRAW;
			maxProofSearchAttempts = SimulationSettings.Default.MAX_PROOF_SEARCH_ATTEMPTS;
			if (assumptions.Count > 0 && goals.Count > 0)
			{
				lock (goals) // TODO: Why this lock is necessary?
				{
					foreach (string goal in goals)
					{
						for (int i = 0; i < maxProofSearchAttempts; ++i)
						{
							drawsCounter = 0;
							do currAssumptions = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
							while (!IsGoalWellMatchedWithAssumptionsCheck(goal, currAssumptions) && ++drawsCounter < maxDraws);
							isProofFound = prover.SearchForProof(currAssumptions, goal);
							if (isDebugModeOn || isProofFound)
							{
								tmpPacket = new ProvenPacket(dictionary.HashId, currAssumptions, goal, prover.GetPartialOutput());
								if (isDebugModeOn)
								{
									LogCurrentStateAsDebug(tmpPacket);
								}
								if (isProofFound)
								{
									this.Facts.Add(tmpPacket);
									break;
								}
							}
						}
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
	}
}
