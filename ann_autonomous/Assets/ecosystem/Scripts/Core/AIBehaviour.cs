using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{
	/// <summary>
	/// Base class for each steering behaviour. 
	/// Also has a list of tag and layer names used throughout the project.
	/// </summary>
	[RequireComponent (typeof(MovingAgent))]
	public abstract class AIBehaviour : MonoBehaviour
	{

		// Layer Names.
		public static readonly string WALL_LAYER_NAME = "Wall";

		// Default weight for each behaviour - edit individual weights using inspector.
		public float Weight = 10f;

		protected MovingAgent m_Entity;
		protected Sight m_Sight;

		// If enabled will output debug logs when entities with required tags are not found in scene.
		protected static readonly bool LOGGING_ENABLED = true;

		// Script name used for debug purposes.
		private static readonly string SCRIPT_NAME = typeof(AIBehaviour).Name;

		protected virtual void Awake ()
		{
			m_Sight = GetComponentInChildren<Sight> ();
			m_Entity = GetComponent<MovingAgent> ();

			if (m_Entity == null && LOGGING_ENABLED) {
				Debug.LogError (SCRIPT_NAME + ": Entity Script on object not found");
			}
		}



		public abstract Vector2 GetForce ();



		/// <summary>
		/// Gets entities with tag name that are in scene. Does not take into account sight range.
		/// </summary>
		/// <returns>The entities with specified tag.</returns>
		/// <param name="tagName">Tag name of object.</param>
		/// <param name="scriptName">Script name for debugging.</param>
		/// <param name="loggingEnabled">If set to <c>true</c> debug will be logged if no object found.</param>
		protected GameObject[] GetEntitiesWithTagName (string tagName, string scriptName, bool loggingEnabled)
		{
			var entities = GameObject.FindGameObjectsWithTag (tagName);

			if (loggingEnabled && (entities == null || entities.Length == 0)) {
				Debug.Log (scriptName + ": No GameObject with tag " + tagName + " found");
			}

			return entities;
		}


		protected GameObject GetEntityWithTagName (string tagName, string scriptName, bool loggingEnabled)
		{
			var entity = GameObject.FindGameObjectWithTag (tagName);

			if (!entity && loggingEnabled) {
				Debug.Log (scriptName + ": No GameObject with tag " + tagName + " found");
			}

			return entity;
		}

		/// <summary>
		/// Arrive at the specified targetPos. Decelerates on arrival. Used by a number of
		/// different behaviours.
		/// </summary>
		/// <param name="targetPos">Target position.</param>
		/// <param name="deceleration">Deceleration rate.</param>
		protected Vector2 Arrive (Vector2 targetPos, float deceleration)
		{
			var toTarget = targetPos - (Vector2)transform.position;

			var dist = toTarget.magnitude;

			if (dist > 0) {
				var speed = dist / deceleration;

				speed = Mathf.Min (speed, m_Entity.maxVelocity);

				var desiredVel = toTarget * speed / dist;

				return (desiredVel - m_Entity.velocity);
			}

			return Vector2.zero;

		}
	}
}
