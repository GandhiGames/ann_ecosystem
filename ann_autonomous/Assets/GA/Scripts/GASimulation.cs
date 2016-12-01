using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{
	public class GASimulation : MonoBehaviour
	{
		public int maxPredators;
		public GameObject predatorPrefab;
		public int predatorPoolSize = 20;

		public int maxPrey;
		public GameObject preyPrefab;
		public int preyPoolSize = 10;

		public int maxVegetation;
		public GameObject vegetationPrefab;

		public float mutationRate;
		public float maxPerturbation;

		private List<GeneticAlgorithm> m_Algorithms;

		void Start ()
		{
			m_Algorithms = new List<GeneticAlgorithm> ();

			if (maxPredators > 0) {
				var predators = new List<GAAgent> ();

				for (int i = 0; i < maxPredators; i++) {
    
					var pos = new Vector2 (Random.Range (-10f, 10f), Random.Range (-10f, 10f));
					var agentObj = (GameObject)MonoBehaviour.Instantiate (predatorPrefab, pos, Quaternion.identity);
					var agent = agentObj.GetComponent<GAAgent> ();

					agent.SetNeuralNetwork (new NeuralNet (2, 5, 1, 22));

					predators.Add (agent);
				}

				m_Algorithms.Add (new GeneticAlgorithm (predators, predatorPoolSize, false, 0.5f, predatorPrefab));

			}

			if (maxPrey > 0) {
				var prey = new List<GAAgent> ();

				for (int i = 0; i < maxPrey; i++) {

					var pos = new Vector2 (Random.Range (-10f, 10f), Random.Range (-10f, 10f));
					var agentObj = (GameObject)MonoBehaviour.Instantiate (preyPrefab, pos, Quaternion.identity);
					var agent = agentObj.GetComponent<GAAgent> ();

					agent.SetNeuralNetwork (new NeuralNet (2, 6, 1, 22));

					prey.Add (agent);
				}

				m_Algorithms.Add (new GeneticAlgorithm (prey, preyPoolSize, false, 0.5f, preyPrefab));

			}
		}

		void Update ()
		{
			foreach (var ga in m_Algorithms) {
				ga.DoUpdate ();
			}
		}

	

	}
}