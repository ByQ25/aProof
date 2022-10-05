using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aProof
{
	class Environment
	{
		private Agent[] agents;

		public Environment(string dictionaryPath, int suggestedAgentsNumber)
		{
			DictHandler dict = new DictHandler(dictionaryPath);
			int maxAgentsNumber = (int)SimulationSettings.Default.NUMBER_OF_AGENTS;
			int agentsNumber = suggestedAgentsNumber;

			if (suggestedAgentsNumber < 1 || suggestedAgentsNumber > maxAgentsNumber)
			{
				Random rng = new Random();
				agentsNumber = rng.Next(maxAgentsNumber) + 1;
			}
			this.agents = new Agent[agentsNumber];
			for (int i = 0; i < agents.Length; ++i) agents[i] = new Agent(dict);
		}

		public Environment(string dictionaryPath) : this(dictionaryPath, 0) { }
	}
}
