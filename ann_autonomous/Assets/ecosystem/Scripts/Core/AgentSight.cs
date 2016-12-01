using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{
	public interface Sight
	{
		HashSet<MovingAgent> GetAgentsInSightWithTag (string tag);
	}
		
	public class AgentSight : MonoBehaviour, Sight
	{
		public float sightRadius = 20f;

		private static AgentDatabase AGENT_DB;

		private int m_ID;


		void Awake ()
		{
			m_ID = transform.gameObject.GetInstanceID ();

			if (AGENT_DB == null) {
				AGENT_DB = FindObjectOfType<AgentDatabase> ();
			}
		}

		public HashSet<MovingAgent> GetAgentsInSightWithTag (string tag)
		{
			var allAgents = AGENT_DB.GetAgentsWithTag (tag);

			if (allAgents.Count > 0) {

				var agentsInSight = new HashSet<MovingAgent> ();

				foreach (var agent in allAgents) {

					if (agent.transform.gameObject.GetInstanceID ().Equals (m_ID)) {
						continue;
					}

					float to = (agent.transform.position - transform.position).sqrMagnitude;

					if (to < sightRadius * sightRadius) {
						agentsInSight.Add (agent);
					}
				}

				return agentsInSight;
			}

			return allAgents;
		}

	}
}