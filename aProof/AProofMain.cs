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
			
			// Pojemność relacji będzie określana w słowniku, agenci będą musieli się do niej dostosować. Trzeba będzie przerobić metodę generującą wyrażenie relacyjne. Można wspomnieć o pomyśle udostępnienia agentom pomysłu łamania tej reguły w koncepcjach na rozwój.
			// Jeśli założenia zwierają relacje, to cel musi składać się z relacji określonej w celu.
			// Czy należałoby pokusić się o wielowątkowość w poszukiwaniu dowodu?
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
