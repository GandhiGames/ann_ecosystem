using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{
	public class GASimulation : MonoBehaviour
	{
        [Header("Predators")]
		public int maxPredators;
		public GameObject predatorPrefab;
		public int predatorPoolSize = 20;

        [Header("Prey")]
        public int maxPrey;
		public GameObject preyPrefab;
		public int preyPoolSize = 10;

        [Header("Vegetation")]
        public int maxVegetation;
		public GameObject vegetationPrefab;

        [Header("GA")]
        public float mutationRate;
		public float maxPerturbation;
        public Vector2 minXYSpawn = new Vector2(-15f, 15f);

		private List<GeneticAlgorithm> m_Algorithms;
        public List<GeneticAlgorithm> algorithms { get { return m_Algorithms; } }

        private static AgentDatabase AGENT_DATABASE;

        void Awake()
        {
            if(AGENT_DATABASE == null)
            {
                AGENT_DATABASE = FindObjectOfType<AgentDatabase>();
            }
        }

		void Start ()
		{
			m_Algorithms = new List<GeneticAlgorithm> ();

			if (maxPredators > 0) {
				var predators = new List<GAAgent> ();

				for (int i = 0; i < maxPredators; i++) {
    
					var pos = new Vector2 (Random.Range (minXYSpawn.x, minXYSpawn.y), Random.Range (minXYSpawn.x, minXYSpawn.y));
					var agentObj = (GameObject)Instantiate (predatorPrefab, pos, Quaternion.identity);
					var agent = agentObj.GetComponent<GAAgent> ();

					agent.SetNeuralNetwork (new NeuralNet (36, 2, 1, 22));

					predators.Add (agent);
				}

				m_Algorithms.Add (new GeneticAlgorithm (predators, predatorPoolSize, false, 0.5f, predatorPrefab));

			}

			if (maxPrey > 0) {
				var prey = new List<GAAgent> ();

				for (int i = 0; i < maxPrey; i++) {

                    var pos = new Vector2(Random.Range(minXYSpawn.x, minXYSpawn.y), Random.Range(minXYSpawn.x, minXYSpawn.y));
                    var agentObj = (GameObject)Instantiate (preyPrefab, pos, Quaternion.identity);
					var agent = agentObj.GetComponent<GAAgent> ();

					agent.SetNeuralNetwork (new NeuralNet (36, 2, 1, 22));

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

            int vegToSpawn = NumOfVegetationToSpawn();

            for(int i = 0; i < vegToSpawn; i++)
            {
                var pos = new Vector2(Random.Range(minXYSpawn.x, minXYSpawn.y), Random.Range(minXYSpawn.x, minXYSpawn.y));
                Instantiate(vegetationPrefab, pos, Quaternion.identity);
            }
		}

	    private int NumOfVegetationToSpawn()
        {
            int numOfVeg = AGENT_DATABASE.GetStationaryAgentsWithTag("Vegetation").Count;

            return maxVegetation - numOfVeg;
        }

	}
}