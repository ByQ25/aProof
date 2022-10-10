using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace aProof
{
	class Environment
	{
		private readonly string dictionaryPath = @"..\..\dictionary.csv";
		private readonly string knownFactsFilePath = @"..\..\known_facts.json";
		private readonly DictHandler dictionary;
		private readonly Agent[] agents;
		private readonly HashSet<Agent.ProvenPacket> knownFacts;


		public Environment(int suggestedAgentsNumber)
		{
			this.dictionary = new DictHandler(dictionaryPath);
			this.knownFacts = (HashSet<Agent.ProvenPacket>)LoadProofsFoundByAgents(knownFactsFilePath).Where(pp => pp.DictionaryHashId == dictionary.HashId);
			int maxAgentsNumber = (int)SimulationSettings.Default.NUMBER_OF_AGENTS;
			int agentsNumber = suggestedAgentsNumber;
			int knownFactsPerAgent;
			
			if (suggestedAgentsNumber < 1 || suggestedAgentsNumber > maxAgentsNumber)
			{
				Random rng = new Random();
				agentsNumber = rng.Next(maxAgentsNumber) + 1;
			}
			this.agents = new Agent[agentsNumber];
			knownFactsPerAgent = knownFacts.Count / agents.Length;
			for (int i = 0; i < agents.Length; ++i)
			{
				agents[i] = new Agent(dictionary);
				for (int j = 0; j < knownFactsPerAgent; ++j)
					agents[i].Facts.Add(knownFacts.ElementAt(j + knownFactsPerAgent * i));
			}
		}

		public Environment(string dictionaryPath) : this(0) { }

		private void SaveProofsFoundByAgents(string filePath, Agent[] agents, HashSet<Agent.ProvenPacket> facts)
		{
			if (!File.Exists(filePath))
				try { File.Create(filePath); }
				catch { return; }
			foreach (Agent agent in agents)
				foreach (Agent.ProvenPacket fact in agent.Facts)
					facts.Add(fact);
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
				sw.Write(JsonConvert.SerializeObject(facts));
		}

		private HashSet<Agent.ProvenPacket> LoadProofsFoundByAgents(string filePath)
		{
			HashSet<Agent.ProvenPacket> facts = new HashSet<Agent.ProvenPacket>();
			if (File.Exists(filePath))
				using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
					try { facts = JsonConvert.DeserializeObject<HashSet<Agent.ProvenPacket>>(sr.ReadToEnd()); }
					catch { facts.Clear(); }
			return facts;
		}
	}
}
