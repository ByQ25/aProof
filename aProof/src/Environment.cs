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
		private int progress;
		public int Progress
		{
			get { return progress; }
			private set
			{
				if (value < 0)
					progress = 0;
				else if (value > 100)
					progress = 100;
				else
					progress = value;
			}
		}
		public List<Tuple<uint, string>> ReadyMsgs { get; }

		public Environment(int suggestedAgentsNumber)
		{
			this.progress = 0;
			this.ReadyMsgs = new List<Tuple<uint, string>>();
			this.dictionaryPath = SimulationSettings.Default.DICTIONARY_FILE_PATH;
			this.knownFactsFilePath = SimulationSettings.Default.KNOWN_FACTS_FILE_PATH;
			this.dictionary = new DictHandler(dictionaryPath);
			this.knownFacts = FilterProofsByDictionary(LoadProofsFromJson(knownFactsFilePath), this.dictionary.HashId);
			this.agents = InitializeAgents(suggestedAgentsNumber);
			for (int i = 0; i < this.knownFacts.Count; ++i)
				agents[i % agents.Length].AddExternalKnownFact(this.knownFacts.ElementAt(i), false);
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
				agents[a++] = new Agent(dictionary, rng.Next(), (uint)a);
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
					finally { if (facts == null) facts = new HashSet<ProvenPacket>(); }
			return facts;
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
					agents[a].AddExternalKnownFact(fact, true);
			}
		}

		public void LetAgentsThinkInAdvance(uint iterations)
		{
			Thread[] threads = new Thread[this.agents.Length];
			for (uint i = 0; i < iterations;)
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
				this.Progress = (int)(++i * 100.0 / iterations);
			}
			SaveProofsToJson(this.knownFactsFilePath, this.knownFacts);
		}

		public void CarryConversation(uint iterations, uint thinkingReps, bool shouldFormatOutputAsMessage)
		{
			Agent[] agentsWithFreshFacts = GetAgentsWithFreshFacts();
			Tuple<ProvenPacket, string> factWithMessage = null;
			this.ReadyMsgs.Clear();
			for (int i = 0; i < iterations;)
			{
				while (agentsWithFreshFacts.Length < 1)
				{
					LetAgentsThinkInAdvance(thinkingReps);
					agentsWithFreshFacts = GetAgentsWithFreshFacts();
				}
				for (int a = 0; a < agentsWithFreshFacts.Length; ++a)
				{
					factWithMessage = factWithMessage == null ?
						agentsWithFreshFacts[a].ChooseFactAndSpeak(null)
						: agentsWithFreshFacts[a].ChooseFactAndSpeak(factWithMessage.Item1);
					lock (ReadyMsgs)
					{
						ReadyMsgs.Add(
							new Tuple<uint, string>(
								agentsWithFreshFacts[a].Identity,
								factWithMessage.Item2
							)
						);
					}
					DistributeNewFact(factWithMessage.Item1, 1);
				}
				this.Progress = (int)(++i * 100.0 / iterations);
			}
		}

		public void CarryConversation(uint iterations, uint thinkingReps)
		{
			CarryConversation(iterations, thinkingReps, true);
		}

		public void CarryConversation(uint iterations, bool shouldFormatOutputAsMessage)
		{
			CarryConversation(
				iterations,
				SimulationSettings.Default.DEFAULT_THINKING_ITERATIONS,
				shouldFormatOutputAsMessage
			);
		}

		public void CarryConversation(uint iterations)
		{
			CarryConversation(
				iterations,
				SimulationSettings.Default.DEFAULT_THINKING_ITERATIONS
			);
		}
	}
}
