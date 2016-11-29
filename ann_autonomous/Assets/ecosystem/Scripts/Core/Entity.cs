using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{

    /// <summary>
    /// Gets force from the Behaviour system and applies it to the objects position.
    /// Attach to an entity that has behaviour scripts attached.
    /// </summary>
    [RequireComponent(typeof(BehaviourManager))]
    public class Entity : MonoBehaviour
    {
        public float MaxVelocity = 2f;
        public float Mass = 0.5f;

        // How far the agent can see - objects within this radius will be taken into account when apply behaviours.
        public float SightRadius = 20;

        // Friction the agent experiences travelling.
        public float Friction = 1.01f;

        // Smoothing sums an a number of the agents movement updates. Use this if the agents movement is twitchy.
        public bool smoothing = true;

        public bool lockMovementOnSingleAxis = false;

        public Vector2 heading { get; private set; }
        public Vector2 velocity { get; private set; }


        private BehaviourManager m_BehaviourManager;
        private Smoother m_Smoother;

        /// <summary>
        /// Gets behaviour manager and smoother (if smoothing is enabled) and sets heading to an arbitrary position. 
        /// </summary>
        void Start()
        {
            m_BehaviourManager = GetComponent<BehaviourManager>();

            velocity = Vector2.zero;

            float rotation = Random.Range(0f, 2f) * (Mathf.PI * 2);
            heading = new Vector2((float)Mathf.Sin(rotation), (float)-Mathf.Cos(rotation));

            if (smoothing)
            {

                // If smoother attached then use that.
                m_Smoother = GetComponent<Smoother>();

                // Else add smoother componenet.
                if (!m_Smoother)
                {
                    m_Smoother = gameObject.AddComponent<Smoother>();
                }
            }

        }

        private Vector2 PerformSmootingIfEnabled(Vector2 vel)
        {
            if (smoothing)
            {
                vel = m_Smoother.UpdateHeading(velocity);
            }

            return vel;
        }

        /// <summary>
        /// Gets force from behaviour manager. Updates entities velocity based on force.
        /// Smooths agents velocity (if enabled) and finally applies velocity to agents position.
        /// </summary>
        void Update()
        {
            Vector2 force = m_BehaviourManager.GetForce();

            /*
            if (Mathf.Abs(force.x) > Mathf.Abs(force.y))
            {
                force = new Vector2(force.x, 0f);
            }
            else
            {
                force = new Vector2(0f, force.y);
            }
            */

            UpdateVelocity(force);

            velocity = PerformSmootingIfEnabled(velocity);

            velocity /= Friction;

            //print((double)velocity.x + ":" + (double)velocity.y);
            transform.position += (Vector3)velocity;

        }



        private void UpdateVelocity(Vector2 force)
        {

            Vector2 acceleration = force / Mass;

            velocity += acceleration * Time.deltaTime;
            velocity = Truncate(velocity, MaxVelocity);
            //print((double)velocity.x + ":" + (double)velocity.y);

            if (lockMovementOnSingleAxis)
            {
                if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
                {
                    velocity = new Vector2(velocity.x, 0f);
                }
                else
                {
                    velocity = new Vector2(0f, velocity.y);
                }
            }

            if (velocity.sqrMagnitude > 0.00000001f)
            {
                heading = (velocity * Time.deltaTime).normalized;

                /*
                if (velocity.sqrMagnitude > 0.01f)
                {
                    float angle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                */
            }


        }

        private Vector2 Truncate(Vector2 original, float max)
        {
            if (original.magnitude > max)
            {
                original.Normalize();

                original *= max;
            }

            return original;
        }


    }
}
