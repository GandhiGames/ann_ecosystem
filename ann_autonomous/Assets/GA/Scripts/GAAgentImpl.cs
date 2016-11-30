using UnityEngine;
using System;
using System.Collections.Generic;

namespace Automation
{

	public interface GAAgent : IComparable
	{
		int instanceID { get; }

        GameObject owner { get; }
		Vector2 position { get; }

		bool isAlive { get; }

		int timeAlive { get; }

		bool isAddedToPool { get; set; }

        void SetNeuralNetwork(NeuralNet net);
        NeuralNet GetNeuralNetwork();
        void Mutate();
		void DoUpdate();
		void Disable();
	}

	[RequireComponent(typeof(GAAgentMovement))]
	public class GAAgentImpl : MonoBehaviour, GAAgent
	{
		public float maxEnergy;

        public GameObject owner { get { return gameObject; } }
		public Vector2 position { get { return transform.position; } }

		public int instanceID { get { return gameObject.GetInstanceID (); } }

		public bool isAlive { get { return m_currentEnergy > 0; } }

		public int timeAlive { get; private set; }

		public bool isAddedToPool { get; set; }

        private static readonly float ENERGY_DECREMENT_OFFSET = 1f;


        private static GASimulation m_GA;

        private NeuralNet m_NeuralNet;
        private BehaviourManager m_BehaviourManager;
		private float m_currentEnergy;
		private GAAgentMovement m_Movement;

        private Sight m_Sight;

		void Awake ()
		{
			if (m_GA == null) {
				m_GA = FindObjectOfType<GASimulation> ();
			}

			m_Movement = GetComponent<GAAgentMovement> ();
			m_BehaviourManager = GetComponent<BehaviourManager> ();
            m_Sight = GetComponentInChildren<Sight>();
		}

        void Start()
        {
            m_currentEnergy = maxEnergy;
        }

		public void Reset ()
		{
			m_BehaviourManager.Reset ();
			m_currentEnergy = maxEnergy;
			timeAlive = 0;


			isAddedToPool = false;
		}

		public void Disable()
		{
			gameObject.SetActive (false);
		}

		public void DoUpdate ()
		{
			if (!isAlive || m_NeuralNet == null) {
				return;
			}

			timeAlive++;

			UpdateWeights ();

			Vector2 force = m_BehaviourManager.GetForce ();

			m_Movement.DoMovement (force);

            m_currentEnergy -= m_Movement.velocity.magnitude + ENERGY_DECREMENT_OFFSET * Time.deltaTime;
		}

		public void Mutate ()
		{
			List<float> weights = new List<float> ();
			weights.AddRange (m_NeuralNet.GetWeights ());

			for (int i = 0; i < weights.Count; ++i) {
				if (Utilities.instance.RandomMinMax (0, 1) < m_GA.mutationRate) {
					weights [i] += (Utilities.instance.RandomMinMax (-1, 1) * m_GA.maxPerturbation);
				}
			}

			m_NeuralNet.SetWeights (weights);
		}

		public int CompareTo (object obj)
		{
			if (obj == null) {
				return 1;
			}

			var otherAgent = (GAAgent)obj;

			return otherAgent.timeAlive - this.timeAlive;
		}

		public List<float> GetNetworkWeights ()
		{
			return m_NeuralNet.GetWeights ();
		}

        public void SetNeuralNetwork(NeuralNet net)
        {
            m_NeuralNet = net;
        }

        public NeuralNet GetNeuralNetwork()
        {
            return m_NeuralNet;
        }

        private void UpdateWeights ()
		{
			List<float> neuralNetInput = new List<float> ();
			List<float> outputs = new List<float> ();

            float numOfPred = (float)m_Sight.GetAgentsInSightWithTag("Predator").Count / m_GA.maxPredators;
            float numOfPrey = (float)m_Sight.GetAgentsInSightWithTag("Prey").Count / m_GA.maxPrey;

            neuralNetInput.Add (numOfPred);
			neuralNetInput.Add (numOfPrey);
			//neuralNetInput.Add (m_VegInSight);

			outputs.AddRange (m_NeuralNet.Update (neuralNetInput));

			m_BehaviourManager.SetWeights (outputs);

		}
    }
}