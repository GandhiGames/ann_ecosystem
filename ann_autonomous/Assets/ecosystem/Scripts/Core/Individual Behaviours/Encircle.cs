using UnityEngine;
using System.Collections;

namespace Automation
{
		/// <summary>
		/// Steers to arrive at a specified distance from the player.
		/// </summary>
		public class Encircle : AIBehaviour
		{

				public bool tightCircle = false;

				public string tagName;

				public float distanceFromTarget = 30f;

				private static readonly string SCRIPT_NAME = typeof(Encircle).Name;

				// Use this for initialization
				void Start ()
				{

            if (tagName.Equals(""))
            {
                Debug.LogWarning("No tag name set");
            }

        }
		
				public override Vector2 GetForce ()
				{
						var objToEncircle = GetEntityWithTagName (tagName, SCRIPT_NAME, LOGGING_ENABLED);

						if (!objToEncircle)
								return Vector2.zero;

						if (Vector2.Distance (objToEncircle.transform.position, transform.position) > distanceFromTarget) {
								return Arrive (objToEncircle.transform.position, 1f);
						} else {

								Vector2 retForce = Vector2.zero;

								if (tightCircle)
										retForce = objToEncircle.transform.position - transform.position;
								else
										retForce = transform.position - objToEncircle.transform.position;

								return retForce;
						}
				}
		}
}
