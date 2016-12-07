using UnityEngine;
using System.Collections;

namespace Automation
{
	[RequireComponent(typeof(GAAgentImpl))]
	public class GAPredator : MonoBehaviour
	{
		public int energyIncrementOnPreyContact = 15;
        public int energyDecrementOnPredContact = 10;

		private GAAgent m_Agent;

		void Awake()
		{
			m_Agent = GetComponent<GAAgent> ();
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag ("Prey")) {
				m_Agent.IncrementEnergy (energyIncrementOnPreyContact);
				other.GetComponent<GAAgent> ().Kill ();
			} else if(other.CompareTag("Predator")) {
                m_Agent.IncrementEnergy(-energyDecrementOnPredContact);
            }
		}

        void OnTriggerStay2D(Collider2D other)
        {
            /*
            if (other.CompareTag("Predator"))
            {
                m_Agent.IncrementEnergy(-Time.deltaTime);
            }
            */
        }
    }
}