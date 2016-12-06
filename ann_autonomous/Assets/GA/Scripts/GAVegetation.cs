using UnityEngine;

namespace Automation
{

    public class GAVegetation : MonoBehaviour, SimulatedAgent
    {
        private static AgentDatabase AGENT_DATABASE;

        void Awake()
        {
            AGENT_DATABASE = FindObjectOfType<AgentDatabase>();
        }

        void OnEnable()
        {
            AGENT_DATABASE.Add(this);
        }

        void OnDisable()
        {
            AGENT_DATABASE.Remove(this);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}