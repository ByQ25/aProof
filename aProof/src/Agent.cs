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
		private readonly Random rng;
		private readonly HashSet<string> assumptionsSimple, assumptionsRelational, goalsSimple, goalsRelational;
		private readonly DictHandler dictionary;
		private readonly ProverHelper prover;
		private readonly src.MessageFormatter msgFormatter;
		private readonly HashSet<ProvenPacket> usedFacts, facts;	// Facts = proven goals, initially empty because facts must be proven
		public uint Identity { get; }
		public HashSet<ProvenPacket> Facts { get { return new HashSet<ProvenPacket>(facts); } }
		public int GetNumberOfKnownNewFacts { get { return GetFreshFacts().Count; } } 

		public Agent(DictHandler dictionary, HashSet<string> assumptions, HashSet<string> goals, int rngSeed, uint id)
		{
			this.Identity = id;
			this.isDebugModeOn = SimulationSettings.Default.IS_IN_DEBUG_MODE;
			this.dictionary = dictionary;
			this.assumptionsSimple = new HashSet<string>();
			this.assumptionsRelational = new HashSet<string>();
			this.goalsRelational = new HashSet<string>();
			this.goalsSimple = new HashSet<string>();
			SplitAssumptionsOrGoals(assumptions, ExpressionType.Assumptions);
			SplitAssumptionsOrGoals(goals, ExpressionType.Goals);
			this.rng = new Random(rngSeed);
			this.prover = new ProverHelper();
			this.msgFormatter = new src.MessageFormatter(rngSeed);
			this.usedFacts = new HashSet<ProvenPacket>();
			this.facts = new HashSet<ProvenPacket>();
		}

		public Agent(DictHandler dictionary, int rngSeed, uint id) : this(dictionary, new HashSet<string>(), new HashSet<string>(), rngSeed, id)
		{
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Assumptions);
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Goals);
		}

		public Agent(DictHandler dictionary, int rngSeed) : this(dictionary, rngSeed, (uint)new Random().Next(1, 100)) { }

		private string ReturnSign()
		{
			return rng.Next(100) < 10 ? "-" : "";
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
			string currOperator = "";
			bool
				prevOperatorNeedsBrackets = false,
				currOperatorNeedsBrackets = false;
			StringBuilder output = new StringBuilder(512);
			for (int i = 0; i < wordsCount; ++i)
			{
				currOperator = ReturnOperator(rng.Next(4));
				currOperatorNeedsBrackets = currOperator.Contains("->");
				if (
					i < wordsCount - 1
					&& currOperatorNeedsBrackets
					&& !prevOperatorNeedsBrackets
				) output.Append("(");
				output.Append(GeneratePartialExpr(exprComplexity));
				if (prevOperatorNeedsBrackets)
					output.Append(")");
				if (i < wordsCount - 1)
				{
					output.Append(currOperator);
					if (currOperatorNeedsBrackets
						&& (
							prevOperatorNeedsBrackets
							|| output[output.Length - currOperator.Length - 1] == ')'
							&& output[output.Length - currOperator.Length - 2] == ')'
						)
					) output.Insert(0, '(');
				}
				prevOperatorNeedsBrackets = currOperatorNeedsBrackets;
			}
			output.Append(".");
			return output.ToString();
		}

		private ExpressionComplexity DetrmineExpressionComplexity(string expr)
		{
			if (expr.Contains("all") || expr.Contains("exists"))
				return ExpressionComplexity.GeneralQuantifiers;
			foreach (string relation in this.dictionary.Relations)
				if (expr.Contains(string.Concat(relation, "(")))
					return ExpressionComplexity.Relational;
			return ExpressionComplexity.Simple;
		}

		private void SplitAssumptionsOrGoals(HashSet<string> exprs, ExpressionType exprType)
		{
			foreach (string expr in exprs)
				AddAssumptionOrGoal(expr, DetrmineExpressionComplexity(expr), exprType);
		}

		private void AddAssumptionOrGoal(string expr, ExpressionComplexity exprComplexity, ExpressionType exprType)
		{
			HashSet<string> setRefrenceTmp;
			if (string.IsNullOrEmpty(expr) || expr == ".")
				return;
			switch (exprType)
			{
				case ExpressionType.Assumptions:
					if (exprComplexity == ExpressionComplexity.Simple)
						setRefrenceTmp = this.assumptionsSimple;
					else
						setRefrenceTmp = this.assumptionsRelational;
					break;
				case ExpressionType.Goals:
					if (exprComplexity == ExpressionComplexity.Simple)
						setRefrenceTmp = this.goalsSimple;
					else
						setRefrenceTmp = this.goalsRelational;
					break;
				default:
					setRefrenceTmp = new HashSet<string>();
					break;
			}
			setRefrenceTmp.Add(expr);
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
					case 0:	// Simple expression
						AddAssumptionOrGoal(
							GenerateExpr(wordsCount, ExpressionComplexity.Simple),
							ExpressionComplexity.Simple,
							exprType
						);
						break;				
					case 1: // Relational expression
						AddAssumptionOrGoal(
							GenerateExpr(wordsCount, ExpressionComplexity.Relational),
							ExpressionComplexity.Relational,
							exprType
						);
						break;
					case 2:	// General quantifiers: exists, all
						AddAssumptionOrGoal(
							GenerateExpr(1, ExpressionComplexity.GeneralQuantifiers),
							ExpressionComplexity.GeneralQuantifiers,
							exprType
						);
						break;
				}
			}
		}

		public void RefreshAssumptionsAndGoals()
		{
			HashSet<string>
				randomSimpleAssumptionsToReuse = DrawTemporaryExpressions(this.assumptionsSimple),
				randomRelatinalAssumptionsToReuse = DrawTemporaryExpressions(this.assumptionsRelational),
				randomSimpleGoalsToReuse = DrawTemporaryExpressions(this.goalsSimple),
				randomRelationalGoalsToReuse = DrawTemporaryExpressions(this.goalsRelational);
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Assumptions);
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Goals);
			this.assumptionsSimple.Concat(randomSimpleAssumptionsToReuse);
			this.assumptionsRelational.Concat(randomRelatinalAssumptionsToReuse);
			this.goalsSimple.Concat(randomSimpleGoalsToReuse);
			this.goalsRelational.Concat(randomRelationalGoalsToReuse);
			foreach (ProvenPacket fact in this.facts)
			{
				foreach (string assumption in fact.Assumptions)
					AddAssumptionOrGoal(assumption, DetrmineExpressionComplexity(assumption), ExpressionType.Assumptions);
				AddAssumptionOrGoal(fact.Goal, DetrmineExpressionComplexity(fact.Goal), ExpressionType.Goals);
			}
		}

		private HashSet<string> DrawTemporaryExpressions(HashSet<string> exprSet)
		{
			int exprToExcludeCount = 0;
			List<string> outputSet = new List<string>();
			if (exprSet.Count > 0)
			{
				outputSet = new List<string>(exprSet);
				exprToExcludeCount = rng.Next(outputSet.Count);
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

		public void VerifyAllGoals()
		{
			uint
				maxDraws = SimulationSettings.Default.MAX_REPEATS_FOR_DRAW,
				maxAttempts = SimulationSettings.Default.MAX_PROOF_SEARCH_ATTEMPTS;
			VerifyGivenGoals(this.assumptionsSimple, this.goalsSimple, maxDraws, maxAttempts);
			VerifyGivenGoals(this.assumptionsRelational, this.goalsRelational, maxDraws, maxAttempts);
		}

		public void VerifyGivenGoals(HashSet<string> assumptions, HashSet<string> goals, uint maxDraws, uint maxAttempts)
		{
			uint drawsCounter;
			bool isProofFound;
			ProvenPacket tmpPacket;
			HashSet<string> currAssumptions;
			if (assumptions.Count > 0 && goals.Count > 0)
			{
				foreach (string goal in goals)
				{
					for (int i = 0; i < maxAttempts; ++i)
					{
						drawsCounter = 0;
						do currAssumptions = DrawTemporaryExpressions(assumptions);
						while (!IsGoalWellMatchedWithAssumptionsCheck(goal, currAssumptions) && ++drawsCounter < maxDraws);
						isProofFound = prover.SearchForProof(currAssumptions, goal);
						if (isDebugModeOn || isProofFound)
						{
							tmpPacket = new ProvenPacket(dictionary.HashId, currAssumptions, goal, prover.GetProofInfoOnly());
							if (isDebugModeOn)
							{
								LogCurrentStateAsDebug(tmpPacket);
							}
							if (isProofFound)
							{
								this.facts.Add(tmpPacket);
								break;
							}
						}
					}
				}
			}
		}

		public void TakeSuggestion(string[] suggestedAssumptions, string[] suggestedGoals)
		{
			if (suggestedAssumptions.Count() > 0)
				foreach (string assumption in suggestedAssumptions)
					AddAssumptionOrGoal(assumption, DetrmineExpressionComplexity(assumption), ExpressionType.Assumptions);
			if (suggestedGoals.Count() > 0)
				foreach (string goal in suggestedGoals)
					AddAssumptionOrGoal(goal, DetrmineExpressionComplexity(goal), ExpressionType.Goals);
		}

		public void AddExternalKnownFact(ProvenPacket factToAdd, bool wasUsed)
		{
			foreach (string assumption in factToAdd.Assumptions)
				AddAssumptionOrGoal(assumption, DetrmineExpressionComplexity(assumption), ExpressionType.Assumptions);
			AddAssumptionOrGoal(factToAdd.Goal, DetrmineExpressionComplexity(factToAdd.Goal), ExpressionType.Goals);
			this.facts.Add(factToAdd);
			if (wasUsed)
				this.usedFacts.Add(factToAdd);
		}

		private HashSet<ProvenPacket> GetFreshFacts()
		{
			return new HashSet<ProvenPacket>(this.facts.Except(usedFacts));
		}

		public Tuple<ProvenPacket, string> SayHello()
		{
			return new Tuple<ProvenPacket, string>(new ProvenPacket(), msgFormatter.PrepareMessage());
		}

		public Tuple<ProvenPacket, string> ChooseFactAndSpeak(ProvenPacket? previousFact)
		{
			ProvenPacket chosenFact = new ProvenPacket();
			HashSet<ProvenPacket> stillFreshFacts = GetFreshFacts();
			int
				tmpScore = -1,
				currentBestScore = -1,
				numberOfUsedFacts = this.usedFacts.Count,
				numberOfFreshFacts = stillFreshFacts.Count;
			bool areFactsSimilar = false;
			string message = "";
			if (previousFact.HasValue && previousFact.Value.Goal != null && numberOfFreshFacts >  1 && rng.Next(4) < 3)
			{
				foreach (ProvenPacket fact in stillFreshFacts)
				{
					tmpScore = (int)fact.GetSimilarityScore((ProvenPacket)previousFact);
					if (tmpScore > currentBestScore)
					{
						chosenFact = fact;
						currentBestScore = tmpScore;
					}
				}
				if (currentBestScore >= chosenFact.Assumptions.Count / 2)
					areFactsSimilar = true;
			}
			else
				chosenFact = stillFreshFacts.ElementAt(rng.Next(numberOfFreshFacts));
			message = msgFormatter.PrepareMessage(chosenFact, areFactsSimilar);
			this.usedFacts.Add(chosenFact);
			return new Tuple<ProvenPacket, string>(chosenFact, message);
		}

		private void LogCurrentStateAsDebug(ProvenPacket pp)
		{
			src.CustomErrorHandler.Log(pp.ToString(), src.CustomErrorHandler.LogLevel.DEBUG);
		}

		public class AgentException : ApplicationException
		{
			public AgentException(string errMsg) : base(errMsg) { }
		}
	}
}
