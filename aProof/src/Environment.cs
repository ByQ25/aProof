using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace aProof
{
	class Environment
	{
		private readonly string dictionaryPath;
		private readonly string knownFactsFilePath;
		private readonly DictHandler dictionary;
		private readonly Agent[] agents;
		private readonly HashSet<ProvenPacket> knownFacts;

		public Environment(int suggestedAgentsNumber)
		{
			this.dictionaryPath = SimulationSettings.Default.DICTIONARY_FILE_PATH;
			this.knownFactsFilePath = SimulationSettings.Default.KNOWN_FACTS_FILE_PATH;
			this.dictionary = new DictHandler(dictionaryPath);
			this.knownFacts = FilterProofsByDictionary(LoadProofsFromJson(knownFactsFilePath), this.dictionary.HashId);
			this.agents = InitializeAgents(suggestedAgentsNumber);
			for (int i = 0; i < this.knownFacts.Count; ++i)
				agents[i % agents.Length].AddExternalKnownFact(this.knownFacts.ElementAt(i));
		}

		public Environment(string dictionaryPath) : this(0) { }

		private Agent[] InitializeAgents(int suggestedAgentsNumber)
		{
			Agent[] agents;
			Random rng = new Random();
			int maxAgentsNumber = (int)SimulationSettings.Default.NUMBER_OF_AGENTS;
			int agentsNumber = suggestedAgentsNumber;
			if (suggestedAgentsNumber < 1 || suggestedAgentsNumber > maxAgentsNumber)
				agentsNumber = rng.Next(maxAgentsNumber) + 1;
			agents = new Agent[agentsNumber];
			for (int a = 0; a < agentsNumber;)
				agents[a++] = new Agent(dictionary, rng.Next());
			return agents;
		}

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
			knownFacts.Add(new ProvenPacket(dictionary.HashId, assumptions, goals.First(), "Test string."));
			SaveProofsToJson(knownFactsFilePath, knownFacts);
		}

		private Agent[] GetAgentsWithFreshFacts()
		{
			List<Agent> output = new List<Agent>();
			for (int i = 0; i < agents.Length; ++i)
				if (agents[i].GetNumberOfKnownNewFacts > 0)
					output.Add(agents[i]);
			return output.ToArray();
		}

		private void DistributeNewFact(ProvenPacket fact, int sourceAgentId)
		{
			for (int a = 0; a < agents.Length; ++a)
			{
				if (a == sourceAgentId)
					continue;
				else
					agents[a].AddExternalKnownFact(fact);
			}
		}

		private void WriteMessage(string input)
		{
			throw new NotImplementedException();
		}

		public void LetAgentsThinkInAdvance(uint iterations)
		{
			Thread[] threads = new Thread[this.agents.Length];
			for (uint i = 0; i < iterations; ++i)
			{
				for (int j = 0; j < threads.Length; ++j)
				{
					threads[j] = new Thread(new ThreadStart(agents[j].VerifyAllGoals));
					threads[j].Start();
				}
				for (int k = 0; k < threads.Length; ++k)
					threads[k].Join();
				GatherFactsFoundByAgents();
				for (int a = 0; a < agents.Length; ++a)
					agents[a].RefreshAssumptionsAndGoals();
				Console.WriteLine(i); // TODO: Remove this line befeore release
			}
			SaveProofsToJson(this.knownFactsFilePath, this.knownFacts);
		}

		public void CarryConversation(uint iterations)
		{
			Agent[] agentsWithFreshFacts = GetAgentsWithFreshFacts();
			Tuple<ProvenPacket, string> factWithMessage;
			for (int i = 0; i < iterations; ++i)
			{
				while (agentsWithFreshFacts.Length < 1)
				{
					LetAgentsThinkInAdvance(SimulationSettings.Default.DEFAULT_THINKING_ITERATIONS);
					agentsWithFreshFacts = GetAgentsWithFreshFacts();
				}
				for (int a = 0; a < agentsWithFreshFacts.Length; ++a)
				{
					factWithMessage = agentsWithFreshFacts[a].ChooseFactAndSpeak();
					DistributeNewFact(factWithMessage.Item1, 1);
					WriteMessage(factWithMessage.Item2);
				}
			}
			throw new NotImplementedException();
		}
	}
}
