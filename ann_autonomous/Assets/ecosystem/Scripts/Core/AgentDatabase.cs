using UnityEngine;
using System.Collections.Generic;

namespace Automation
{
	public class AgentDatabase : MonoBehaviour
	{
        private readonly static HashSet<MovingAgent> EMPTY_MOVING_LIST = new HashSet<MovingAgent>();
        private readonly static HashSet<StationaryAgent> EMPTY_STATIONARY_LIST = new HashSet<StationaryAgent>();

        private Dictionary<string, HashSet<MovingAgent>> m_AgentsInSimulation = new Dictionary<string, HashSet<MovingAgent>> ();
        private Dictionary<string, HashSet<StationaryAgent>> m_StationaryAgentsInSimulation = new Dictionary<string, HashSet<StationaryAgent>>();


        public void Add (MovingAgent agent)
		{
			var tag = agent.transform.gameObject.tag;

			if (!m_AgentsInSimulation.ContainsKey (tag)) {
				m_AgentsInSimulation.Add (tag, new HashSet<MovingAgent> ());
			}

			m_AgentsInSimulation [tag].Add (agent);
		}

        public void Add(StationaryAgent agent)
        {
            var tag = agent.transform.gameObject.tag;

            if (!m_StationaryAgentsInSimulation.ContainsKey(tag))
            {
                m_StationaryAgentsInSimulation.Add(tag, new HashSet<StationaryAgent>());
            }

            m_StationaryAgentsInSimulation[tag].Add(agent);
        }

		public void Remove (MovingAgent agent)
		{
			var tag = agent.transform.gameObject.tag;

			if (!m_AgentsInSimulation.ContainsKey (tag)) {
				return;
			}

			m_AgentsInSimulation [tag].Remove (agent);
		}

        public void Remove(StationaryAgent agent)
        {
            var tag = agent.transform.gameObject.tag;

            if (!m_StationaryAgentsInSimulation.ContainsKey(tag))
            {
                return;
            }

            m_StationaryAgentsInSimulation[tag].Remove(agent);
        }

        public HashSet<MovingAgent> GetMovingAgentsWithTag (string tag)
		{
			if (m_AgentsInSimulation.ContainsKey (tag)) {
				return m_AgentsInSimulation [tag];
			}

			if (EMPTY_MOVING_LIST.Count > 0) {
				EMPTY_MOVING_LIST.Clear ();
			}

			return EMPTY_MOVING_LIST;

		}

        public HashSet<StationaryAgent> GetStationaryAgentsWithTag(string tag)
        {
            if (m_StationaryAgentsInSimulation.ContainsKey(tag))
            {
                return m_StationaryAgentsInSimulation[tag];
            }

            if (EMPTY_MOVING_LIST.Count > 0)
            {
                EMPTY_MOVING_LIST.Clear();
            }

            return EMPTY_STATIONARY_LIST;

        }
    }
}