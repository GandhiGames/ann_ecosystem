using UnityEngine;
using System.Collections;

namespace Automation
{
	public class GAAgentMovement : MonoBehaviour
	{
		public float maxVelocity = 2f;
		public float mass = 5f;

		// Friction the agent experiences travelling.
		public float friction = 1.01f;

		public bool lockMovementOnSingleAxis = false;
		// Smoothing sums an a number of the agents movement updates. Use this if the agents movement is twitchy.
		public bool smoothing = true;

		public Vector2 velocity { get; private set; }

		public Vector2 heading { get; private set; }

		private Smoother m_Smoother;

		void Awake ()
		{
			if (smoothing) {

				// If smoother attached then use that.
				m_Smoother = GetComponent<Smoother> ();

				// Else add smoother componenet.
				if (!m_Smoother) {
					m_Smoother = gameObject.AddComponent<Smoother> ();
				}
			}
		}

		public void Reset ()
		{
			velocity = Vector2.zero;
		}

		public void DoMovement (Vector2 force)
		{
			UpdateVelocity (force);

			velocity = PerformSmootingIfEnabled (velocity);

			velocity /= friction;

			transform.position += (Vector3)velocity;
		}

		private Vector2 PerformSmootingIfEnabled (Vector2 vel)
		{
			if (smoothing) {
				vel = m_Smoother.UpdateHeading (velocity);
			}

			return vel;
		}

		private void UpdateVelocity (Vector2 force)
		{

			Vector2 acceleration = force / mass;

			velocity += acceleration * Time.deltaTime;
			velocity = Truncate (velocity, maxVelocity);

			if (lockMovementOnSingleAxis) {
				if (Mathf.Abs (velocity.x) > Mathf.Abs (velocity.y)) {
					velocity = new Vector2 (velocity.x, 0f);
				} else {
					velocity = new Vector2 (0f, velocity.y);
				}
			}

			if (velocity.sqrMagnitude > 0.00000001f) {
				heading = (velocity * Time.deltaTime).normalized;

			}


		}

		private Vector2 Truncate (Vector2 original, float max)
		{
			if (original.magnitude > max) {
				original.Normalize ();

				original *= max;
			}

			return original;
		}
	}
}