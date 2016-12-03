using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{
    public interface StationaryAgent
    {
        Transform transform { get; }
    }

    public interface MovingAgent
    {
        float maxVelocity { get; }
        Vector2 velocity { get; }
        Vector2 heading { get; }
		Transform transform { get; }
    }

    /// <summary>
    /// Gets force from the Behaviour system and applies it to the objects position.
    /// Attach to an entity that has behaviour scripts attached.
    /// </summary>
    [RequireComponent(typeof(BehaviourManager))]
    public class Entity : MonoBehaviour, MovingAgent
    {

        [SerializeField]
        private float m_MaxVelocity = 2f;
        public float maxVelocity { get { return m_MaxVelocity; } }
        public float Mass = 0.5f;

        // Friction the agent experiences travelling.
        public float Friction = 1.01f;

        // Smoothing sums an a number of the agents movement updates. Use this if the agents movement is twitchy.
        public bool smoothing = true;

        public bool lockMovementOnSingleAxis = false;

		public bool rotateTowardsHeading = false;

        public Vector2 heading { get; private set; }
        public Vector2 velocity { get; private set; }
		//public Transform ownerTransform { get { return transform; } }

        private BehaviourManager m_BehaviourManager;
        private Smoother m_Smoother;

		private static AgentDatabase AGENT_DB;

		void Awake()
		{
			if (AGENT_DB == null) {
				AGENT_DB = FindObjectOfType<AgentDatabase> ();
			}

			m_BehaviourManager = GetComponent<BehaviourManager>();

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

        /// <summary>
        /// Gets behaviour manager and smoother (if smoothing is enabled) and sets heading to an arbitrary position. 
        /// </summary>
        void Start()
        {
   	       	velocity = Vector2.zero;

            float rotation = Random.Range(0f, 2f) * (Mathf.PI * 2);
            heading = new Vector2((float)Mathf.Sin(rotation), (float)-Mathf.Cos(rotation));
        }

		void OnEnable()
		{
			AGENT_DB.Add (this);
		}

		void OnDisable()
		{
			AGENT_DB.Remove (this);
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

            UpdateVelocity(force);

            velocity = PerformSmootingIfEnabled(velocity);

            velocity /= Friction;

            transform.position += (Vector3)velocity;
        }



        private void UpdateVelocity(Vector2 force)
        {

            Vector2 acceleration = force / Mass;

            velocity += acceleration * Time.deltaTime;
            velocity = Truncate(velocity, maxVelocity);

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

                
				if (rotateTowardsHeading)
                {
                    float angle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                
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
