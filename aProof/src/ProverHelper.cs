using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace aProof
{
	class ProverHelper
	{
		private string proverPath;
		private static readonly string
			defaultProverPath = @"..\..\Prover9-Mace4\bin-win32\prover9.exe";
		private IProverOutputProcessor outputProcessor;


		public ProverHelper(string proverPath)
		{
			this.proverPath = proverPath;
			this.outputProcessor = new ProverOutputProcessor();
		}

		public ProverHelper() : this(defaultProverPath) { }

		private string PrepareInput(HashSet<string> assumptions, HashSet<string> goals)
		{
			StringBuilder inputFormatted = new StringBuilder(4096);
			inputFormatted.Append("formulas(assumptions).\n");
			foreach (string assumption in assumptions)
				inputFormatted.Append(assumption);
			inputFormatted.Append("\nend_of_list.\nformulas(goals).\n");
			foreach (string goal in goals)
				inputFormatted.Append(goal);
			inputFormatted.Append("\nend_of_list.");
			return inputFormatted.ToString();
		}

		public bool SearchForProof(string input)
		{
			StringBuilder
				stdOutput = new StringBuilder(4096),
				stdError = new StringBuilder(4096);
			int
				maxSearchTimeInSec = (int)SimulationSettings.Default.MAX_PROOF_SEARCH_TIME,
				maxSearchTimeInMiliSec = 1000 * maxSearchTimeInSec;
			using (Process proverProc = new Process())
			{
				proverProc.StartInfo = new ProcessStartInfo
				{
					FileName = proverPath,
					Arguments = string.Concat("-t ", maxSearchTimeInSec),
					CreateNoWindow = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden
				};
				using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
				using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
				{
					proverProc.OutputDataReceived += (sender, e) =>
					{
						if (e.Data == null)
							outputWaitHandle.Set();
						else
							stdOutput.AppendLine(e.Data);
					};
					proverProc.ErrorDataReceived += (sender, e) =>
					{
						if (e.Data == null)
							errorWaitHandle.Set();
						else
							stdError.AppendLine(e.Data);
					};
					proverProc.Start();
					proverProc.StandardInput.Write(input);
					proverProc.StandardInput.Close();
					proverProc.BeginOutputReadLine();
					proverProc.BeginErrorReadLine();
					if (
						!proverProc.WaitForExit(maxSearchTimeInMiliSec)
						|| !outputWaitHandle.WaitOne(maxSearchTimeInMiliSec)
						|| !errorWaitHandle.WaitOne(maxSearchTimeInMiliSec)
					)
					{
						proverProc.CancelOutputRead();
						proverProc.CancelErrorRead();
						proverProc.Kill();
					}
				}
			}
			outputProcessor.Reset();
			return outputProcessor.ProcessProverOutput(stdOutput.ToString());
		}

		public bool SearchForProof(HashSet<string> assumptions, HashSet<string> goals)
		{
			return SearchForProof(PrepareInput(assumptions, goals));
		}

		public bool SearchForProof(HashSet<string> assumptions, string goal)
		{
			return SearchForProof(assumptions, new HashSet<string>() { goal });
		}

		public string GetProofInfoOnly()
		{
			return outputProcessor.Proof.ToString();
		}

		public string GetPartialOutput()
		{
			StringBuilder output = new StringBuilder(4096);
			output.Append("Input:\n");
			output.Append(outputProcessor.Input);
			output.Append("\n\nProof:\n");
			output.Append(outputProcessor.Proof);
			if (outputProcessor.IsProofFound)
				output.Append(
					string.Format("\nFact proven. Number of proofs: {0}\n", outputProcessor.NumberOfProofs)
				);
			else output.Append("\nNo proof.\n");
			return output.ToString();
		}

		public string GetFullOutput()
		{
			StringBuilder output = new StringBuilder(16384);
			output.Append("Header:\n");
			output.Append(outputProcessor.Header);
			output.Append("\n\nInput:\n");
			output.Append(outputProcessor.Input);
			output.Append("\n\nProcess Clauses:\n");
			output.Append(outputProcessor.ProcessClauses);
			output.Append("\n\nPredicate Elimination:\n");
			output.Append(outputProcessor.PredicateElimination);
			output.Append("\n\nSearch:\n");
			output.Append(outputProcessor.Search);
			output.Append("\n\nProof:\n");
			output.Append(outputProcessor.Proof);
			output.Append("\n\nStatistics:\n");
			output.Append(outputProcessor.Statistics);
			output.Append("\n\nAdditional:\n");
			output.Append(outputProcessor.Additional);
			return output.ToString();
		}

		public class ProverHelperException : ApplicationException
		{
			public ProverHelperException(string errMsg) : base(errMsg) { }
		}
	}
}
