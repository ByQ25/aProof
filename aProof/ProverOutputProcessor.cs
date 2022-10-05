using System.Text;
using System.Collections.Generic;

namespace aProof
{
	class ProverOutputProcessor : IProverOutputProcessor
	{
		private enum ProofSection
		{
			Header,
			Input,
			ProcessClauses,
			PredicateElimination,
			ClausesForSearch,
			Search,
			Proof,
			Statistics,
			Additional
		}

		private readonly Dictionary<ProofSection, string> availableSectionsDict = new Dictionary<ProofSection, string>()
		{
			{ ProofSection.Header, "Prover9" },
			{ ProofSection.Input, "INPUT" },
			{ ProofSection.ProcessClauses, "PROCESS" },
			{ ProofSection.PredicateElimination, "PREDICATE ELIMINATION" },
			{ ProofSection.ClausesForSearch, "CLAUSES FOR SEARCH" },
			{ ProofSection.Search, "SEARCH" },
			{ ProofSection.Proof, "PROOF" },
			{ ProofSection.Statistics, "STATISTICS" },
			{ ProofSection.Additional, "" }
		};

		public StringBuilder Header { get; }
		public StringBuilder Input { get; }
		public StringBuilder ProcessClauses { get; }
		public StringBuilder PredicateElimination { get; }
		public StringBuilder Search { get; }
		public StringBuilder Proof { get; }
		public StringBuilder Statistics { get; }
		public StringBuilder Additional { get; }
		public bool IsProofFound { get; private set; }
		public uint NumberOfProofs { get; private set; }

		public ProverOutputProcessor()
		{
			this.Header = new StringBuilder(256);
			this.Input = new StringBuilder(2048);
			this.ProcessClauses = new StringBuilder(2048);
			this.PredicateElimination = new StringBuilder(2048);
			this.Search = new StringBuilder(2048);
			this.Proof = new StringBuilder(2048);
			this.Statistics = new StringBuilder(1024);
			this.Additional = new StringBuilder(1024);
			this.IsProofFound = false;
		}

		public ProverOutputProcessor(string proof) : this()
		{
			ProcessProverOutput(proof);
		}

		public bool ProcessProverOutput(string proof)
		{
			StringBuilder currentSection = this.Header;
			string[] splittedProof = proof.Split('\n');

			foreach (string line in splittedProof)
			{
				if (line.Length > 0)
				{
					switch (line[0])
					{
						case '%': continue;
						case '=':
							if (line.Contains(availableSectionsDict[ProofSection.Header])) currentSection = this.Header;
							else if (line.Contains(availableSectionsDict[ProofSection.Input])) currentSection = this.Input;
							else if (line.Contains(availableSectionsDict[ProofSection.ProcessClauses])) currentSection = this.ProcessClauses;
							else if (line.Contains(availableSectionsDict[ProofSection.PredicateElimination])) currentSection = this.PredicateElimination;
							else if (line.Contains(availableSectionsDict[ProofSection.Search])) currentSection = this.Search;
							else if (line.Contains(availableSectionsDict[ProofSection.Proof])) currentSection = this.Proof;
							else if (line.Contains(availableSectionsDict[ProofSection.Statistics])) currentSection = this.Statistics;
							else currentSection = this.Additional;
							break;
						default: currentSection.AppendLine(line); break;
					}
				}
			}

			return DetermineResult(splittedProof);
		}

		private bool DetermineResult(string[] input)
		{
			for (int i = input.Length - 1; i > -1; i--)
			{
				if (input[i].Contains("Exiting with"))
				{
					string proofsNumber = "";
					for (int j = 0; j < input[i].Length; j++)
						if (char.IsDigit(input[i][j])) proofsNumber += input[i][j];
					this.NumberOfProofs = uint.TryParse(proofsNumber, out uint number) ? number : 0;
				}
				if (input[i].Contains("SEARCH FAILED"))
				{
					this.IsProofFound = false;
					break;
				}
				if (input[i].Contains("THEOREM PROVED"))
				{
					this.IsProofFound = true;
					break;
				}
			}
			return this.IsProofFound;
		}

		public void Reset()
		{
			this.Header.Clear();
			this.Input.Clear();
			this.PredicateElimination.Clear();
			this.Search.Clear();
			this.Proof.Clear();
			this.Statistics.Clear();
			this.Additional.Clear();
			this.IsProofFound = false;
			this.NumberOfProofs = 0;
		}
	}
}
