﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace aProof
{
	class Agent
	{
		private enum ExpressionType { Assumptions, Goals }
		private enum ExpressionComplexity { Simple, Relational, GeneralQuantifiers }
		private Random rng;
		private HashSet<string> assumptions, goals;
		private readonly DictHandler dictionary;
		private readonly ProverHelper prover;
		public HashSet<ProvenPacket> Facts { get; } // Facts = proven goals, initially empty because facts must be proven

		public Agent(DictHandler dictionary, HashSet<string> assumptions, HashSet<string> goals)
		{
			this.dictionary = dictionary;
			this.assumptions = assumptions;
			this.goals = goals;
			this.rng = new Random();
			this.prover = new ProverHelper();
			this.Facts = new HashSet<ProvenPacket>();
			VerifyGoals();
		}

		public Agent(DictHandler dictionary) : this(dictionary, new HashSet<string>(), new HashSet<string>())
		{
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Assumptions);
			DrawInitialAssumptionsOrGoals(dictionary, ExpressionType.Goals);
			VerifyGoals();
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
			return rng.Next(2) == 0 ?
						dictionary.Vars[rng.Next(dictionary.Vars.Length)]
						: dictionary.Nouns[rng.Next(dictionary.Nouns.Length)];
		}

		private string GenerateRelationalSubExpr()
		{
			StringBuilder output = new StringBuilder(256);
			if (dictionary.Relations.Length > 0 && (dictionary.Vars.Length > 0 || dictionary.Nouns.Length > 0))
			{
				KeyValuePair<string, uint> relation = dictionary.RelationsWithSizes.ElementAt(rng.Next(dictionary.RelationsWithSizes.Count));
				output.Append(relation.Key);
				output.Append("(");
				for (int i = 0; i < relation.Value; ++i)
				{
					output.Append(ReturnSign());
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
				if (exprComplexity != ExpressionComplexity.GeneralQuantifiers) output.Append(ReturnSign());
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

		private void VerifyGoals()
		{
			if (assumptions.Count > 0 && goals.Count > 0)
			{
				HashSet<string> currAssumptions = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
				foreach (string goal in goals)
				{
					currAssumptions = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
					//for (int i = 0; i < SimulationSettings.Default.MAX_PROOF_SEARCH_ATTEMPTS; ++i)
					for (int i = 0; i < 1000; ++i)
						if (prover.SearchForProof(currAssumptions, goal))
						{
							this.Facts.Add(new ProvenPacket(dictionary.HashId, assumptions, goal, prover.GetPartialOutput()));
							break;
						}
						else currAssumptions = DrawTemporaryAssumptionsOrGoals(ExpressionType.Assumptions);
				}
			}
		}

		// TODO: Remove testing method before release
		public List<string> TestAgent()
		{
			List<string> result = new List<string>();

			assumptions.Add("mother(Liz, Charley).");
			assumptions.Add("father(Charley, Billy).");
			assumptions.Add("-mother(x, y) | parent(x, y).");
			assumptions.Add("-father(x, y) | parent(x, y).");
			assumptions.Add("-parent(x, y) | ancestor(x, y).");
			assumptions.Add("-parent(x, y) | -ancestor(y, z) | ancestor(x, z).");
			goals.Add("ancestor(Liz, Billy).");
			if (dictionary.ValidateInputAgainstDictionary(assumptions) && dictionary.ValidateInputAgainstDictionary(goals))
			{
				prover.SearchForProof(assumptions, goals);
				result.Add(prover.GetPartialOutput());
				result.Add(prover.GetFullOutput());
				return result;
			}
			else throw new AgentException("Input jest niezgodny z zadanym słownikiem.");
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
		}
	}
}
