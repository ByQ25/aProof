using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace aProof
{
	class Environment
	{
		private readonly string dictionaryPath = @"..\..\dictionary.csv";
		private readonly string knownFactsFilePath = @"..\..\known_facts.json";
		private readonly DictHandler dictionary;
		private readonly Agent[] agents;
		private readonly HashSet<ProvenPacket> knownFacts;


		public Environment(int suggestedAgentsNumber)
		{
			this.dictionary = new DictHandler(dictionaryPath);
			this.knownFacts = FilterProofsByDictionary(LoadProofsFromJson(knownFactsFilePath), this.dictionary.HashId);
			int maxAgentsNumber = (int)SimulationSettings.Default.NUMBER_OF_AGENTS;
			int agentsNumber = suggestedAgentsNumber;
			if (suggestedAgentsNumber < 1 || suggestedAgentsNumber > maxAgentsNumber)
			{
				Random rng = new Random();
				agentsNumber = rng.Next(maxAgentsNumber) + 1;
			}
			this.agents = new Agent[agentsNumber];
			for (int a = 0; a < agentsNumber;)
				agents[a++] = new Agent(dictionary);
			for (int i = 0; i < this.knownFacts.Count; ++i)
				agents[i % agents.Length].AddExternalKnownFact(this.knownFacts.ElementAt(i));
		}

		public Environment(string dictionaryPath) : this(0) { }

		private void GatherFactsFoundByAgents()
		{
			foreach (Agent agent in agents)
				foreach (ProvenPacket fact in agent.Facts)
					knownFacts.Add(fact);
		}

		private HashSet<ProvenPacket> FilterProofsByDictionary(HashSet<ProvenPacket> facts, string dictionaryHash)
		{
			HashSet<ProvenPacket> filteredProofs = new HashSet<ProvenPacket>();
			if (facts.Count > 0)
				foreach (ProvenPacket fact in facts)
					if (fact.DictionaryHashId == dictionaryHash)
						filteredProofs.Add(fact);
			return filteredProofs;
		}

		private void SaveProofsToJson(string filePath, HashSet<ProvenPacket> facts)
		{
			if (!File.Exists(filePath))
				try { File.Create(filePath).Close(); }
				catch { return; }
			using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
				sw.Write(JsonConvert.SerializeObject(facts));
		}

		private HashSet<ProvenPacket> LoadProofsFromJson(string filePath)
		{
			HashSet<ProvenPacket> facts = new HashSet<ProvenPacket>();
			if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
				using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
					try { facts = JsonConvert.DeserializeObject<HashSet<ProvenPacket>>(sr.ReadToEnd()); }
					catch { facts.Clear(); }
			return facts;
		}

		// TODO: Delete before release
		public void TestSerializationToJson()
		{
			HashSet<string> assumptions = new HashSet<string>();
			HashSet<string> goals = new HashSet<string>();
			assumptions.Add("mother(Liz, Charley).");
			assumptions.Add("father(Charley, Billy).");
			assumptions.Add("-mother(x, y) | parent(x, y).");
			assumptions.Add("-father(x, y) | parent(x, y).");
			assumptions.Add("-parent(x, y) | ancestor(x, y).");
			assumptions.Add("-parent(x, y) | -ancestor(y, z) | ancestor(x, z).");
			goals.Add("ancestor(Liz, Billy).");
			knownFacts.Clear();
			knownFacts.Add(new ProvenPacket(dictionary.HashId, assumptions, goals.ElementAt(0), "Test string."));
			SaveProofsToJson(knownFactsFilePath, knownFacts);
		}

		public void LetAgentsThinkInAdvance()
		{
			foreach (Agent agent in agents)
				agent.VerifyGoals();
			// SaveProofsFoundByAgents(this.knownFactsFilePath, this.agents);
			// throw new NotImplementedException();
		}

		public void CarryConversation()
		{
			throw new NotImplementedException();
		}
	}
}
