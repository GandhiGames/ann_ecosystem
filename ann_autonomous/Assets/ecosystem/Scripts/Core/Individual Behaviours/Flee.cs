using UnityEngine;
using System.Collections;

namespace Automation
{
	/// <summary>
	/// Steers the entity away from the player, if the player is within sight range.
	/// </summary>
	public class Flee : AIBehaviour
	{
		public string tagName;


		void Start ()
		{
			if (tagName.Equals ("")) {
				Debug.LogWarning ("No tag name set");
			}
		}

		public override Vector2 GetForce ()
		{
			var player = m_Sight.GetMovingAgentsInRangeWithTag (tagName);

			if (player.Count == 0) {
				return Vector2.zero;
			}

			return (Vector2)transform.position - (Vector2)player.GetEnumerator ().Current.transform.position;
		}
	}
}
