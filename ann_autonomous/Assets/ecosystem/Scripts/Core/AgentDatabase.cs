using UnityEngine;
using System.Collections.Generic;

namespace Automation
{
	public class AgentDatabase : MonoBehaviour
	{
		private Dictionary<string, HashSet<MovingAgent>> m_AgentsInSimulation = new Dictionary<string, HashSet<MovingAgent>> ();
		private readonly static HashSet<MovingAgent> EMPTY_LIST = new HashSet<MovingAgent> ();

		public void Add (MovingAgent agent)
		{
			var tag = agent.transform.gameObject.tag;

			if (!m_AgentsInSimulation.ContainsKey (tag)) {
				m_AgentsInSimulation.Add (tag, new HashSet<MovingAgent> ());
			}

			m_AgentsInSimulation [tag].Add (agent);
		}

		public void Remove (MovingAgent agent)
		{
			var tag = agent.transform.gameObject.tag;

			if (!m_AgentsInSimulation.ContainsKey (tag)) {
				return;
			}

			m_AgentsInSimulation [tag].Remove (agent);
		}

		public HashSet<MovingAgent> GetAgentsWithTag (string tag)
		{
			if (m_AgentsInSimulation.ContainsKey (tag)) {
				return m_AgentsInSimulation [tag];
			}

			if (EMPTY_LIST.Count > 0) {
				EMPTY_LIST.Clear ();
			}

			return EMPTY_LIST;

		}
	}
}