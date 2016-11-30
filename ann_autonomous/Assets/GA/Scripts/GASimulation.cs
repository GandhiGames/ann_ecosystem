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

        public int maxVegetation;
        public GameObject vegetationPrefab;

		public float mutationRate;
		public float maxPerturbation;

        private GeneticAlgorithm m_Algorithm;

        void Start()
        {
            if(maxPredators > 0)
            {
                var predators = new List<GAAgent>();

                for (int i = 0; i < maxPredators; i++)
                {
    
                    var pos = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
                    var agentObj = (GameObject)MonoBehaviour.Instantiate(predatorPrefab, pos, Quaternion.identity);
                    var agent = agentObj.GetComponent<GAAgent>();

                    agent.SetNeuralNetwork(new NeuralNet(2, 4, 1, 22));

                    predators.Add(agent);

                   
                }

                m_Algorithm = new GeneticAlgorithm(predators, predatorPoolSize, false, 0.5f, predatorPrefab);

            }
        }

        void Update()
        {
            m_Algorithm.DoUpdate();
        }

	}
}