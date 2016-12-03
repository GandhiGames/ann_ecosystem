using UnityEngine;
using System.Collections;

namespace Automation
{
    /// <summary>
    /// Group Behaviour.
    /// 
    /// Aligns an entities heading with other entities within it's sight radius.
    /// </summary>
    public class Alignment : AIBehaviour
    {
        public string tagName;

        void Start()
        {
            if (tagName.Equals(""))
            {
                Debug.LogWarning("No tag name set");
            }
        }


        /// <summary>
        /// Iterates through entities in sight range, sums the entities headings and returns the average.
        /// </summary>
        /// <returns>The alignment force.</returns>
        public override Vector2 GetForce()
        {

            Vector2 averageHeading = Vector2.zero;

            var entities = m_Sight.GetMovingAgentsInRangeWithTag(tagName);

            foreach (var obj in entities)
            {
                averageHeading += obj.heading;
            }


            if (entities.Count > 0)
            {
                averageHeading /= entities.Count;
                averageHeading -= m_Entity.heading;
            }

            return averageHeading;
        }

    }
}
