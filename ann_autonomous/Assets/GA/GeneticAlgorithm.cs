using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{

	public class GeneticAlgorithm
	{
		private List<GAAgent> m_Agents = new List<GAAgent> ();
		private List<GAAgent> m_AgentPool = new List<GAAgent> ();
		private int m_AgentsCreated = 0;

		private int m_NumSpawnedFromPool = 0;

		private float m_TotalFitnessScore = 0f;

		private bool m_RandomCrossoverPoint;
		private int m_PoolSize;
		private int m_MaxAgents;

		// Use this for initialization
		public GeneticAlgorithm (int maxAgents, GameObject prefab, int poolsize, bool randomCrossoverPoint)
		{
			m_PoolSize = poolsize;
			m_MaxAgents = maxAgents;
			m_RandomCrossoverPoint = randomCrossoverPoint;
			
			for (int i = 0; i < maxAgents; i++) {
				var pos = new Vector2 (Random.Range (-10f, 10f), Random.Range (-10f, 10f));
				var agent = (GameObject)MonoBehaviour.Instantiate (prefab, pos, Quaternion.identity);
				m_Agents.Add (agent.GetComponent<GAAgent> ());
			}
		}

		public void DoUpdate ()
		{
			for (int i = 0; i < m_Agents.Count; i++) {
				if (m_Agents [i].isAlive) {
					m_Agents [i].DoUpdate ();
				} else {
					UpdatePool (m_Agents [i]);

					CalculateTotalFitness ();


				

					if (m_AgentPool.Count > 1) {
						var parentOne = FitnessProportionateSelection ();
						var parentTwo = FitnessProportionateSelection ();

						if (!parentOne.Equals (parentTwo)) {
							CreateAgentFromCrossover (parentOne, parentTwo, pred, i);
						}

					}


				}
			}
		}

		private void UpdatePool (GAAgent agent)
		{
			if (!agent.isAddedToPool) {
				agent.Disable ();
				m_AgentPool.Add (agent);
				m_AgentPool.Sort ();

				if (m_AgentPool.Count > m_PoolSize) {
					m_AgentPool.RemoveAt (m_AgentPool.Count - 1);
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

		public NeuralNet CrossOver(GAAgent parentOne, GAAgent parentTwo)
		{
			NeuralNet neuralNet = new NeuralNet(false);
			List<float> newWeights = new List<float>();
			List<float> parentOneWeights = parentOne.networkWeights;
			List<float> parentTwoWeights = parentTwo.networkWeights;

			int crossOverPoint;

			if (m_RandomCrossoverPoint) {
				crossOverPoint = (int)Utilities.RandomMinMax (0, parentOneWeights.Count);
			} else {
				crossOverPoint = (int)(parentOneWeights.Count * 0.5f);
			}

			for (int i = 0; i < crossOverPoint; i++)
			{
				newWeights.Add(parentOneWeights[i]);
			}

			for (int i = crossOverPoint; i < parentOneWeights.Count; i++)
			{
				newWeights.Add(parentTwoWeights[i]);
			}

			neuralNet.SetWeights(ref newWeights);

			return neuralNet;
		}

		private GAAgent FitnessProportionateSelection()
		{
			float randomSlice = Utilities.RandomMinMax(0, m_TotalFitnessScore);

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
			}

			return choosenAgent;
		}

		private void CreateAgentFromCrossover(GAAgent parentOne, GAAgent parentTwo, int index)
		{

			NeuralNet neuralNetwork = CrossOver(parentOne, parentTwo);

			var pos = new Vector2 (Random.Range (-10f, 10f), Random.Range (-10f, 10f));
			var agent = (GameObject)MonoBehaviour.Instantiate (prefab, pos, Quaternion.identity);
			m_Agents.Add (agent.GetComponent<GAAgent> ());

			m_Agents[index] = new Predator(predTexture, new Vector2(tempX, tempY),
				Utilities.AgentTextureSize, Utilities.Mass, Utilities.SightRadiusPredator,
				Utilities.LateralLinePred, neuralNetwork);


			if (Utilities.RandomMinMax(0, 1) < Utilities.MutationChance)
			{
				pred[i].Mutate();
			}


			m_AgentsCreated++;

			UpdateGenerationNumber();

		}


	}
}