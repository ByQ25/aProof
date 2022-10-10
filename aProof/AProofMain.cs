using System;

namespace aProof
{
	class AProofMain
	{
		static void Main(string[] args)
		{
			Environment env = new Environment(2);
			Console.ReadKey(true);
		}

		public class AProofException : ApplicationException
		{
			public AProofException(string errMsg) : base(errMsg) { }
		}
	}
}
