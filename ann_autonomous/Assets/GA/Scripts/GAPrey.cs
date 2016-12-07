using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Automation
{
    [RequireComponent(typeof(GAAgentImpl))]
    public class GAPrey : MonoBehaviour
    {
        public int energyIncrementOnVegContact = 15;
        public int energyDecrementOnPreyContact = 5;

        private GAAgent m_Agent;

        void Awake()
        {
            m_Agent = GetComponent<GAAgent>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Vegetation"))
            {
                m_Agent.IncrementEnergy(energyIncrementOnVegContact);
                other.GetComponent<GAVegetation>().Remove();
            }
            else if (other.CompareTag("Prey"))
            {
                m_Agent.IncrementEnergy(-energyDecrementOnPreyContact);
            }

        }
    }
}