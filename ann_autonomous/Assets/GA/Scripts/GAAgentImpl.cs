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

		float timeAlive { get; }

		bool isAddedToPool { get; set; }

        void SetNeuralNetwork(NeuralNet net);
        NeuralNet GetNeuralNetwork();
        void Mutate();
		void DoUpdate();
		void Disable();
		void Kill();
		void IncrementEnergy(int amount);
	}

	[RequireComponent(typeof(GAAgentMovement))]
	public class GAAgentImpl : MonoBehaviour, GAAgent
	{
		public float maxEnergy;

        public GameObject owner { get { return gameObject; } }
		public Vector2 position { get { return transform.position; } }

		public int instanceID { get { return gameObject.GetInstanceID (); } }

		public bool isAlive { get { return m_currentEnergy > 0; } }

		public float timeAlive { get; private set; }

		public bool isAddedToPool { get; set; }

        private static readonly float ENERGY_DECREMENT_OFFSET = 1f;

        private static GASimulation m_GA;

        private NeuralNet m_NeuralNet;
        //private BehaviourManager m_BehaviourManager;
		private float m_currentEnergy;
		private GAAgentMovement m_Movement;

        private Sight m_Sight;

		void Awake ()
		{
			if (m_GA == null) {
				m_GA = FindObjectOfType<GASimulation> ();
			}

			m_Movement = GetComponent<GAAgentMovement> ();
            m_Sight = GetComponentInChildren<Sight>();
		}

        void Start()
        {
            m_currentEnergy = maxEnergy;
        }

        /*
		public void Reset ()
		{
            //m_BehaviourManager.Reset ();
            m_Force = Vector2.zero;
			m_currentEnergy = maxEnergy;
			timeAlive = 0;


			isAddedToPool = false;
		}
        */

		public void Disable()
		{
			gameObject.SetActive (false);
		}

		public void Kill()
		{
			m_currentEnergy = 0;
		}

		public void IncrementEnergy(int amount)
		{
			m_currentEnergy = Mathf.Min (m_currentEnergy + amount, maxEnergy);
		}

		public void DoUpdate ()
		{
			if (!isAlive || m_NeuralNet == null) {
				return;
			}

			timeAlive += Time.deltaTime;

			var force = GetForce ();

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

            if(otherAgent.timeAlive > this.timeAlive)
            {
                return -1;
            }
            else if(otherAgent.timeAlive < this.timeAlive)
            {
                return 1;
            }
            else
            {
                return 0;
            }
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

        private Vector2 GetForce ()
		{
            List<float> neuralNetInput = new List<float>();

            neuralNetInput.AddRange (GetWeights<MovingAgent>(m_Sight.GetMovingAgentsInRangeWithTag("Prey")));
			neuralNetInput.AddRange (GetWeights<MovingAgent>(m_Sight.GetMovingAgentsInRangeWithTag("Predator")));
            neuralNetInput.AddRange (GetWeights<SimulatedAgent>(m_Sight.GetStationaryAgentsInRangeWithTag("Vegetation")));

            List<float> outputs = m_NeuralNet.Update(neuralNetInput);

            // output in range [0..1] this converts range to [-0.5..0.5] used for turning left or right.
            float turnForce = outputs[0] - 0.5f;
            float velocityMulti = outputs[1];

            return m_Movement.heading.Rotate((m_Movement.maxTurnAngle * 2f) * turnForce) * (velocityMulti * m_Movement.maxVelocity);
		}

        

        private float[] GetWeights<T>(HashSet<T> agentsInSight) where T : SimulatedAgent
        {
            var input = new float[12];

            foreach (var agent in agentsInSight)
            {

                var forward = m_Movement.heading;
                var right = new Vector2(forward.y, -forward.x);

                Vector3 to = agent.transform.position - transform.position;
                float angle = Vector2.Angle(to, forward);

                // Determine if the degree value should be negative.  Here, a positive value
                // from the dot product means that our vector is on the right of the reference vector   
                // whereas a negative value means we're on the left.
                float sign = Mathf.Sign(Vector2.Dot(to, right));

                // Converted to signed and then normalise in the range positive 0 to sight range.
                float finalAngle = (sign * angle) + (m_Sight.radius * 0.5f);

                if (finalAngle > m_Sight.radius || finalAngle < 0f)
                {
                    //print("Not in sight: " + finalAngle);
                    continue;
                }

                int step = (int)m_Sight.radius / input.Length;

                //print("Angle " + finalAngle);
                //print("step: " + step);

                for (int i = 0; i < input.Length; i++)
                {
                    //print((step * i) + " to " + (step * (i + 1)) );
                    if (finalAngle >= step * i && finalAngle <= step * (i + 1))
                    {
                        // print("storing : " + i);
                        input[i] = 1;
                    }
                    else
                    {
                        input[i] = 0;
                    }

                }
            }

            return input;
        }

    }
}