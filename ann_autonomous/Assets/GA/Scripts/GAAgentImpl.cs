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

		public int timeAlive { get; private set; }

		public bool isAddedToPool { get; set; }

        private static readonly float ENERGY_DECREMENT_OFFSET = 2f;


        private static GASimulation m_GA;

        private NeuralNet m_NeuralNet;
        //private BehaviourManager m_BehaviourManager;
		private float m_currentEnergy;
		private GAAgentMovement m_Movement;

        private Sight m_Sight;

        private Vector2 m_Force;

		void Awake ()
		{
			if (m_GA == null) {
				m_GA = FindObjectOfType<GASimulation> ();
			}

			m_Movement = GetComponent<GAAgentMovement> ();
			//m_BehaviourManager = GetComponent<BehaviourManager> ();
            m_Sight = GetComponentInChildren<Sight>();
		}

        void Start()
        {
            m_currentEnergy = maxEnergy;
        }

		public void Reset ()
		{
            //m_BehaviourManager.Reset ();
            m_Force = Vector2.zero;
			m_currentEnergy = maxEnergy;
			timeAlive = 0;


			isAddedToPool = false;
		}

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

			timeAlive++;

			UpdateWeights ();

			//Vector2 force = m_BehaviourManager.GetForce ();

			m_Movement.DoMovement (m_Force);

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
            //float numOfPred = (float)m_Sight.GetAgentsInRangeWithTag("Predator").Count / m_GA.maxPredators;
            //float numOfPrey = (float)m_Sight.GetAgentsInRangeWithTag("Prey").Count / m_GA.maxPrey;

            var preyInRange = m_Sight.GetAgentsInRangeWithTag("Prey");
            var preyInput = GetWeights(preyInRange);

            var predInRange = m_Sight.GetAgentsInRangeWithTag("Predator");
            var predInput = GetWeights(predInRange);

            List<float> neuralNetInput = new List<float>();

            neuralNetInput.AddRange (preyInput);
			neuralNetInput.AddRange (predInput);
            //neuralNetInput.Add (m_VegInSight);

            List<float> outputs = new List<float>();
            outputs.AddRange (m_NeuralNet.Update (neuralNetInput));


            // 2 output: left/right * speed
            outputs[0] -= 0.5f;
            m_Force = m_Movement.heading.Rotate((m_Movement.maxTurnAngle * 2f) * outputs[0]) * (outputs[1] * m_Movement.maxVelocity);
		}

        private float[] GetWeights(HashSet<MovingAgent> agentsInSight)
        {
            var input = new float[12];

            for (int i = 0; i < input.Length; i++)
            {
                input[i] = 0;
            }

            foreach (var prey in agentsInSight)
            {

                Vector3 referenceForward = m_Movement.heading;
                Vector3 referenceRight = new Vector2(referenceForward.y, -referenceForward.x); // Vector3.Cross(Vector2.up, referenceForward);

                // the vector of interest
                Vector3 newDirection = prey.transform.position - transform.position;
                float angle2 = Vector2.Angle(newDirection, referenceForward);

                // Determine if the degree value should be negative.  Here, a positive value
                // from the dot product means that our vector is on the right of the reference vector   
                // whereas a negative value means we're on the left.
                float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
                // print("sign: " + sign);
                float finalAngle = sign * angle2;

                // print(finalAngle);

                /*

                var dirToPrey = prey.transform.position - transform.position;

                //var heading = (Vector2)transform.position + m_Movement.heading) - (Vector2)transform.position;
                float angleToHeading = Mathf.Atan2(m_Movement.heading.y, m_Movement.heading.x) * Mathf.Rad2Deg;

                float angle = Mathf.Atan2(dirToPrey.y, dirToPrey.x) * Mathf.Rad2Deg - angleToHeading;

                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 100f);

                */

                /*
                if(angle <= 0f && angle >= -90f) // right
                {
                    print("right?");
                }

                if (angle >= 0f && angle <= 90f) // left
                {
                    print("left?");
                }
                */

                finalAngle += 90f;

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

                }

         
            }

            return input;
        }

    
    }
}