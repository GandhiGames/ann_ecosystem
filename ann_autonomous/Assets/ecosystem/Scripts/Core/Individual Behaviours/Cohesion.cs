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

        // Script name - used in GetEntitiesInSight for debugging.
        private static readonly string SCRIPT_NAME = typeof(Cohesion).Name;

        void Start()
        {
            Initialise();

            if (tagName.Equals(""))
            {
                tagName = ENEMY_TAG_NAME;
            }
        }

        /// <summary>
        /// Get all entities within sight that have a tag of ENEMY_TAG_NAME and returns average of positions.
        /// </summary>
        /// <returns>The cohesion force.</returns>
        public override Vector2 GetForce()
        {
            Vector2 centreOfMass = Vector2.zero;

            var entities = GetEntitiesInSight(entity.SightRadius, tagName, SCRIPT_NAME, LOGGING_ENABLED);

            foreach (var obj in entities)
            {
                centreOfMass += (Vector2)obj.transform.position;
            }

            if (entities.Count > 0)
            {
                centreOfMass /= entities.Count;
            }

            return decelerateToTarget ? Arrive(centreOfMass, 1f) : centreOfMass - (Vector2)transform.position;
        }


    }
}
