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

        private static readonly string SCRIPT_NAME = typeof(Seperation).Name;

        void Start()
        {
            Initialise();

            if (tagName.Equals(""))
            {
                tagName = ENEMY_TAG_NAME;
            }
        }

        public override Vector2 GetForce()
        {

            var steeringForce = Vector2.zero;

            var entities = GetEntitiesInSight(entity.SightRadius, tagName, SCRIPT_NAME, LOGGING_ENABLED);

            foreach (var obj in entities)
            {
                Vector2 toAgent = transform.position - obj.transform.position;
                steeringForce += toAgent.normalized / (toAgent.magnitude * MAG_OFFSET);
            }

            return steeringForce;
        }
    }
}
