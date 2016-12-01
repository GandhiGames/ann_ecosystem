using UnityEngine;
using System.Collections;

namespace Automation
{
	[RequireComponent(typeof(GAAgentImpl))]
	public class GAPredator : MonoBehaviour
	{
		public int energyIncrementOnPreyContact = 15;
		
		private GAAgent m_Agent;

		void Awake()
		{
			m_Agent = GetComponent<GAAgent> ();
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag ("Prey")) {
				print ("here");
				m_Agent.IncrementEnergy (energyIncrementOnPreyContact);
				other.GetComponent<GAAgent> ().Kill ();
			}
		}
	}
}