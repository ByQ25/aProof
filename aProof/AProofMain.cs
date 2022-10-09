using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace aProof
{
	class AProofMain
	{
		private static readonly string dictionaryPath = @"..\..\dictionary.csv";

		static void Main(string[] args)
		{
			Agent a1 = new Agent();

			foreach (string output in a1.TestAgent())
				Console.WriteLine(output);
			
			/*
			DictHandler dict = new DictHandler(dictionaryPath);
			Agent a1 = new Agent(dict);
			*/
			Console.ReadKey(true);
		}

		public class AProofException : ApplicationException
		{
			public AProofException(string errMsg) : base(errMsg) { }
		}
	}
}
