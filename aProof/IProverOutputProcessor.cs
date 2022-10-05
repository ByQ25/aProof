using System.Text;

namespace aProof
{
	interface IProverOutputProcessor
	{
		// Fields
		StringBuilder Header { get; }
		StringBuilder Input { get; }
		StringBuilder ProcessClauses { get; }
		StringBuilder PredicateElimination { get; }
		StringBuilder Search { get; }
		StringBuilder Proof { get; }
		StringBuilder Statistics { get; }
		StringBuilder Additional { get; }
		bool IsProofFound { get; }
		uint NumberOfProofs { get; }

		// Methods
		bool ProcessProverOutput(string proof);
	}
}
