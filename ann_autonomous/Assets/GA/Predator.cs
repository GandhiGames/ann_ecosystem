using UnityEngine;
using System;
using System.Collections.Generic;

namespace Automation
{

	public interface GAAgent : IComparable
	{
		int instanceID { get; }

		Vector2 position { get; }

		bool isAlive { get; }

		int timeAlive { get; }

		bool isAddedToPool { get; set; }

		List<float> networkWeights { get; }

		void DoUpdate();
		void Disable();
	}

	[RequireComponent(typeof(GAAgentMovement))]
	public class Predator : MonoBehaviour, GAAgent
	{
		public float maxEnergy;
		public float sightRadius;
		public float lateralRadius;

		public Vector2 position { get { return transform.position; } }

		public int instanceID { get { return gameObject.GetInstanceID (); } }

		public bool isAlive { get { return m_currentEnergy > 0; } }

		public NeuralNet neuralNet { get; set; }

		public int timeAlive { get; private set; }

		public bool isAddedToPool { get; set; }

		public List<float> networkWeights { get { return neuralNet.GetWeights (); } }

		private static GASimulation m_GA;

		private BehaviourManager m_BehaviourManager;
		private float m_currentEnergy;
		private List<GAAgent> m_PreyInSight = new List<GAAgent> ();
		private List<GAAgent> m_PredInSight = new List<GAAgent> ();
		private GAAgentMovement m_Movement;

		void Awake ()
		{
			if (m_GA == null) {
				m_GA = FindObjectOfType<GASimulation> ();
			}

			m_Movement = GetComponent<GAAgentMovement> ();
			m_BehaviourManager = GetComponent<BehaviourManager> ();
		}

		public void Reset ()
		{
			m_BehaviourManager.Reset ();
			m_currentEnergy = maxEnergy;
			timeAlive = 0;

			m_PreyInSight.Clear ();
			m_PredInSight.Clear ();

			isAddedToPool = false;
		}

		public void Disable()
		{
			gameObject.SetActive (false);
		}

		public void DoUpdate (List<GAAgent> prey, List<GAAgent> predators)
		{
			if (!isAlive) {
				return;
			}

			timeAlive++;

			m_PredInSight.Clear ();
			m_PreyInSight.Clear ();

			UpdateWeights (prey, predators);

			Vector2 force = m_BehaviourManager.GetForce ();

			m_Movement.DoMovement (force);

			m_currentEnergy -= (m_Movement.velocity.magnitude * 0.2f) + 0.2f;

		}

		public void Mutate ()
		{
			List<float> weights = new List<float> ();
			weights.AddRange (neuralNet.GetWeights ());

			// int mutate = (int)Utilities.RandomMinMax(0, weights.Count);
			// weights[mutate] += (Utilities.RandomMinMax(-1, 1) * Utilities.MaxPerturbation);

			for (int i = 0; i < weights.Count; ++i) {
				if (Utilities.instance.RandomMinMax (0, 1) < m_GA.mutationRate) {
					weights [i] += (Utilities.instance.RandomMinMax (-1, 1) * m_GA.maxPerturbation);
				}
			}

			neuralNet.SetWeights (ref weights);
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
			return neuralNet.GetWeights ();
		}

		private void UpdateWeights (List<GAAgent> prey, List<GAAgent> pred)
		{
			List<float> neuralNetInput = new List<float> ();
			List<float> outputs = new List<float> ();

			m_PreyInSight.AddRange (GetAgentsInSight (prey));
			m_PredInSight.AddRange (GetAgentsInSight (pred));

			float numOfPreyInSight = m_PreyInSight.Count / m_GA.maxPrey;
			float numOfPredInSight = m_PredInSight.Count / m_GA.maxPredators;

			neuralNetInput.Add (numOfPreyInSight);
			neuralNetInput.Add (numOfPredInSight);
			//neuralNetInput.Add(Energy / maxEnergy);

			outputs.AddRange (neuralNet.Update (neuralNetInput));

			m_BehaviourManager.SetWeights (outputs);

		}



		private List<GAAgent> GetAgentsInSight (List<GAAgent> agents)
		{
			var localAgents = new List<GAAgent> ();
			for (int i = 0; i < agents.Count; i++) {

				if (this.gameObject.GetInstanceID () == agents [i].instanceID) {
					Debug.Log ("Matching");
				} else {
					Vector2 to = agents [i].position - (Vector2)transform.position;

					if (to.sqrMagnitude < (sightRadius * sightRadius)) {
						localAgents.Add (agents [i]);
					}
				}
			}

			return localAgents;
		}
	}
}