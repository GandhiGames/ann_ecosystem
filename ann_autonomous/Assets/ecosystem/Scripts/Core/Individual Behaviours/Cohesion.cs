using UnityEngine;
using System.Collections;

namespace Automation
{

	/// <summary>
	/// Group Behaviour.
	/// 
	/// Provides a steering force towards the center of any entities within sight.
	/// </summary>
	public class Cohesion : AIBehaviour
	{
		public string tagName;
		public bool decelerateToTarget = true;

		void Start ()
		{

			if (tagName.Equals ("")) {
				Debug.LogWarning ("No tag name set");
			}
		}

		/// <summary>
		/// Get all entities within sight that have a tag of ENEMY_TAG_NAME and returns average of positions.
		/// </summary>
		/// <returns>The cohesion force.</returns>
		public override Vector2 GetForce ()
		{
			Vector2 centreOfMass = Vector2.zero;

			var entities = m_Sight.GetAgentsInRangeWithTag (tagName);
                
               

			foreach (var obj in entities) {
				centreOfMass += (Vector2)obj.transform.position;
			}

			if (entities.Count > 0) {
				centreOfMass /= entities.Count;
			}

			return decelerateToTarget ? Arrive (centreOfMass, 1f) : centreOfMass - (Vector2)transform.position;
		}


	}
}
