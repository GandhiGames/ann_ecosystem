using UnityEngine;
using System.Collections.Generic;

namespace Automation
{
    /// <summary>
    /// Loads all attached scripts that inherit from AIBehaviour.
    /// Iterates over each behaviour and combines their force multipled by their respective weights.
    /// </summary>
    public class BehaviourManager : MonoBehaviour
    {
        private AIBehaviour[] m_Behaviours;
        private List<float> m_InitialWeights;

        // Use this for initialization
        void Start()
        {
            GetBehaviours();

            m_InitialWeights = new List<float>();

            foreach(var behaviour in m_Behaviours)
            {
                m_InitialWeights.Add(behaviour.Weight);
            }
        }

        private void GetBehaviours()
        {
            m_Behaviours = gameObject.GetComponents<AIBehaviour>();

            if (m_Behaviours == null || m_Behaviours.Length == 0)
            {
                Debug.LogError("Entity: No Behaviour Scripts Attached to Object " + gameObject.name);
            }

        }


        public Vector2 GetForce()
        {
            Vector2 force = Vector2.zero;

            foreach (var behaviour in m_Behaviours)
            {

                if (behaviour.enabled)
                {
                    force += behaviour.GetForce() * behaviour.Weight;
                }
            }

            return force;

        }

        public void SetWeights(List<float> weights)
        {
            if(weights.Count != m_Behaviours.Length)
            {
                Debug.LogError("Weight count not matching");
            }

            for(int i = 0; i < weights.Count; i++)
            {
                m_Behaviours[i].Weight = weights[i];
            }


        }

        public void Reset()
        {
            for (int i = 0; i < m_Behaviours.Length; i++)
            {
                m_Behaviours[i].Weight = m_InitialWeights[i];
            }
        }
    }
}
