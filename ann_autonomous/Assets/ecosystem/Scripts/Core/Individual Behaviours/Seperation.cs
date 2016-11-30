using UnityEngine;
using System.Collections;

namespace Automation
{
    /// <summary>
    /// Group Behaviour.
    /// 
    /// Provides a steering force away from the center of any entities within sight.
    /// </summary>
    public class Seperation : AIBehaviour
    {
        public string tagName;

        private static readonly float MAG_OFFSET = 1f;

        void Start()
        {

            if (tagName.Equals(""))
            {
                Debug.LogWarning("No tag name set");
            }
        }

        public override Vector2 GetForce()
        {
            var steeringForce = Vector2.zero;

            var entities = m_Sight.GetAgentsInSightWithTag(tagName);

            foreach (var obj in entities)
            {
                Vector2 toAgent = (Vector2)transform.position - obj.position;
                steeringForce += toAgent.normalized / (toAgent.magnitude * MAG_OFFSET);
            }


            return steeringForce;
        }
    }
}
