using UnityEngine;
using System.Collections.Generic;

namespace Automation
{

	public class GeneticAlgorithm
	{
        public int generationNumber { get; private set; }

		private List<GAAgent> m_Agents;
		private List<GAAgent> m_AgentPool = new List<GAAgent> ();
		//private int m_AgentsCreated = 0;

		private int m_NumSpawnedFromPool;

		private float m_TotalFitnessScore = 0f;
		private bool m_RandomCrossoverPoint;
		private int m_MaxPoolSize;
		private float m_MutationChance;

		private GameObject m_AgentPrefab;

		//ANN
		private int m_AnnInput;
		private int m_AnnOutput;
		private int m_AnnHiddenLayers;
		private int m_AnnNeuronsPerHiddenLayer;

		private GameObject pool;

		// Use this for initialization
		public GeneticAlgorithm (List<GAAgent> initialAgents, 
		                         int poolsize, bool randomCrossoverPoint, float mutationChance,
		                         GameObject agentPrefab)
		{
			m_MaxPoolSize = poolsize;
			m_RandomCrossoverPoint = randomCrossoverPoint;
			m_MutationChance = Mathf.Clamp01 (mutationChance);
			m_Agents = initialAgents;
			m_NumSpawnedFromPool = 0;
			generationNumber = 0;

			m_AnnInput = initialAgents [0].GetNeuralNetwork ().numOfInput;
			m_AnnOutput = initialAgents [0].GetNeuralNetwork ().numOfOutput;
			m_AnnHiddenLayers = initialAgents [0].GetNeuralNetwork ().numOfHiddenLayers;
			m_AnnNeuronsPerHiddenLayer = initialAgents [0].GetNeuralNetwork ().numOfNeuronsPerHiddenLayer;

			m_AgentPrefab = agentPrefab;
			m_TotalFitnessScore = 0;

			pool = new GameObject (agentPrefab.name +  " Pool");

		}

		public void DoUpdate ()
		{
			for (int i = 0; i < m_Agents.Count; i++) {
				if (m_Agents [i].isAlive) {

               

					m_Agents [i].DoUpdate ();
				} else {

                 

                    UpdatePool (m_Agents [i], i);

					if (m_AgentPool.Count > 1) {

						CalculateTotalFitness ();

						var parentOne = FitnessProportionateSelection ();
						var parentTwo = FitnessProportionateSelection ();

						if (!parentOne.Equals (parentTwo)) {
                            if (pool.name.Equals("Prey Pool"))
                            {
                                Debug.Log("Adding from pool " + i);
                            }
                            CreateAgentFromCrossover (parentOne, parentTwo, i);
						} 

					}


				}
			}
		}

		private void UpdatePool (GAAgent agent, int i)
		{
			if (!agent.isAddedToPool) {

                if (pool.name.Equals("Prey Pool"))
                {
                    Debug.Log("Removing " + i);
                }

                agent.Disable ();
				agent.isAddedToPool = true;

				m_AgentPool.Add (agent);
				m_AgentPool.Sort ();

				agent.owner.transform.SetParent (pool.transform);

				if (m_AgentPool.Count > m_MaxPoolSize) {
					MonoBehaviour.Destroy (m_AgentPool [m_AgentPool.Count - 1].owner);
					m_AgentPool.RemoveAt (m_AgentPool.Count - 1);
				}

                if (pool.name.Equals("Prey Pool"))
                {
                    Debug.Log("Pool size " + m_AgentPool.Count);

                }

            }
        }

		private void CalculateTotalFitness ()
		{
			m_TotalFitnessScore = 0;

			for (int c = 0; c < m_AgentPool.Count - 1; c++) {
				m_TotalFitnessScore += m_AgentPool [c].timeAlive;
			}
		}

		public NeuralNet CrossOver (GAAgent parentOne, GAAgent parentTwo)
		{
			NeuralNet neuralNet = new NeuralNet (m_AnnInput, m_AnnOutput, m_AnnHiddenLayers, m_AnnNeuronsPerHiddenLayer);
			List<float> newWeights = new List<float> ();
			List<float> parentOneWeights = parentOne.GetNeuralNetwork ().GetWeights ();
			List<float> parentTwoWeights = parentTwo.GetNeuralNetwork ().GetWeights ();

			int crossOverPoint;

			if (m_RandomCrossoverPoint) {
				crossOverPoint = (int)Utilities.instance.RandomMinMax (0, parentOneWeights.Count);
			} else {
				crossOverPoint = (int)(parentOneWeights.Count * 0.5f);
			}

			for (int i = 0; i < crossOverPoint; i++) {
				newWeights.Add (parentOneWeights [i]);
			}

			for (int i = crossOverPoint; i < parentOneWeights.Count; i++) {
				newWeights.Add (parentTwoWeights [i]);
			}

			neuralNet.SetWeights (newWeights);

			return neuralNet;
		}

		private GAAgent FitnessProportionateSelection ()
		{
			float randomSlice = Utilities.instance.RandomMinMax (0, m_TotalFitnessScore);

			GAAgent choosenAgent = null;

			float fitnessTotal = 0;

            for (int i = 0; i < m_AgentPool.Count; i++)
            {
                fitnessTotal += m_AgentPool[i].timeAlive;

                if (fitnessTotal > randomSlice)
                {
                    choosenAgent = m_AgentPool[i];
                    break;
                }

                if (pool.name.Equals("Prey Pool"))
                {
                    Debug.Log("Chose " + i + " of " + m_AgentPool.Count);
                }
            }

			return choosenAgent;
		}

		private void CreateAgentFromCrossover (GAAgent parentOne, GAAgent parentTwo, int index)
		{
			NeuralNet neuralNetwork = CrossOver (parentOne, parentTwo);

			var pos = new Vector2 (Random.Range (-10f, 10f), Random.Range (-10f, 10f));
			var agent = (GameObject)MonoBehaviour.Instantiate (m_AgentPrefab, pos, Quaternion.identity);

			m_Agents [index] = agent.GetComponent<GAAgent> ();

			m_Agents [index].SetNeuralNetwork (neuralNetwork);

			if (Utilities.instance.RandomMinMax (0f, 1f) < m_MutationChance) {
				m_Agents [index].Mutate ();
			}

			UpdateGenerationNumber ();

		}

		private void UpdateGenerationNumber ()
		{
			m_NumSpawnedFromPool++;
			if (m_NumSpawnedFromPool % m_MaxPoolSize == 0) {
				generationNumber++;
			}
		}


	}
}