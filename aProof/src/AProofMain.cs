using System;

namespace aProof
{
	class AProofMain
	{
		static void Main(string[] args)
		{
			char key;
			Environment env = new Environment(2);
			Console.Title = "aProof";
			Console.WriteLine("The environment with agents has been initialized.\n");
			Console.WriteLine("Please select an action to execute by pressing corresponding numeric key:");
			Console.WriteLine("1. Do the thinking only\n2. Carry a conversation between agents\n3. Quit\n");
			do
			{
				key = Console.ReadKey(true).KeyChar;
				switch (key)
				{
					case '1': env.LetAgentsThinkInAdvance(); break;
					case '2': env.CarryConversation(); break;
				}
			} while (key != '3');

		}

		public class AProofException : ApplicationException
		{
			public AProofException(string errMsg) : base(errMsg) { }
		}
	}
}
