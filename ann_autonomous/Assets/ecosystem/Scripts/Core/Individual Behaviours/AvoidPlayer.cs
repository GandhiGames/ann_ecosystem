using UnityEngine;
using System.Collections;

namespace Automation
{

		/// <summary>
		/// Provides a seperation force from any entity within sight with tag equal to PLAYER_TAG_NAME.
		/// </summary>
		public class AvoidPlayer : AIBehaviour
		{

        public string tagName;

				// Increases the strength of the force - lower numbers equals a stronger force.
				private static readonly float MAG_OFFSET = 0.1f;

        void Start()
        {
            if(tagName.Equals(""))
            {
                Debug.LogWarning("No tag name set");
            }
        }

				/// <summary>
				/// Iterates through any objects in sight that have a tag of PLAYER_TAG_NAME and returns a seperation force.
				/// </summary>
				/// <returns>The seperation force.</returns>
				public override Vector2 GetForce ()
				{
						Vector2 steeringForce = Vector2.zero;

            var entities = m_Sight.GetAgentsInSightWithTag(tagName);
                
			
						foreach (var obj in entities) {
								Vector2 toAgent = (Vector2)transform.position - obj.position;
								steeringForce += toAgent.normalized / (toAgent.magnitude * MAG_OFFSET);
						}
			
						return steeringForce;
				}
		}
}
