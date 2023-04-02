using System;
using System.Windows.Forms;

namespace aProof
{
	class AProofMain
	{
		static void Main()
		{
			Application.Run(new MainForm());
		}

		public class AProofException : ApplicationException
		{
			public AProofException(string errMsg) : base(errMsg) { }
		}
	}
}
