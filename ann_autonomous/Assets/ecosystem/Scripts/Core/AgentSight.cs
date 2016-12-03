using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{
	public interface Sight
	{
        float distance { get; }
        float radius { get; }
        HashSet<MovingAgent> GetMovingAgentsInRangeWithTag (string tag);
        HashSet<StationaryAgent> GetStationaryAgentsInRangeWithTag(string tag);
	}

    public class AgentSight : MonoBehaviour, Sight
    {
        public float sightDistance = 20f;

        [Range(0f, 360f)]
        public float sightRadius = 360f;

        public float distance { get { return sightDistance; } }
        public float radius { get { return sightRadius; } }
        private static AgentDatabase AGENT_DB;

        private int m_ID;


        void Awake()
        {
            m_ID = transform.gameObject.GetInstanceID();

            if (AGENT_DB == null)
            {
                AGENT_DB = FindObjectOfType<AgentDatabase>();
            }
        }

        public HashSet<MovingAgent> GetMovingAgentsInRangeWithTag(string tag)
        {
            var allAgents = AGENT_DB.GetMovingAgentsWithTag(tag);

            if (allAgents.Count > 0)
            {

                var agentsInRange = new HashSet<MovingAgent>();

                foreach (var agent in allAgents)
                {

                    if (agent.transform.gameObject.GetInstanceID().Equals(m_ID))
                    {
                        continue;
                    }

                    float to = (agent.transform.position - transform.position).sqrMagnitude;

                    if (to < sightDistance * sightDistance)
                    {
                        agentsInRange.Add(agent);
                    }
                }

                return agentsInRange;
            }

            return allAgents;
        }

        public HashSet<StationaryAgent> GetStationaryAgentsInRangeWithTag(string tag)
        {
            var allAgents = AGENT_DB.GetStationaryAgentsWithTag(tag);

            if (allAgents.Count > 0)
            {

                var agentsInRange = new HashSet<StationaryAgent>();

                foreach (var agent in allAgents)
                {

                    if (agent.transform.gameObject.GetInstanceID().Equals(m_ID))
                    {
                        continue;
                    }

                    float to = (agent.transform.position - transform.position).sqrMagnitude;

                    if (to < sightDistance * sightDistance)
                    {
                        agentsInRange.Add(agent);
                    }
                }

                return agentsInRange;
            }

            return allAgents;


        }
    }
}