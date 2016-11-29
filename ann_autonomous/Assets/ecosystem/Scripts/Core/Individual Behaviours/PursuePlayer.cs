using UnityEngine;
using System.Collections;

namespace Automation
{
		/// <summary>
		/// Attempts to intercept player, taking into account the players heading. 
		/// </summary>
		public class PursuePlayer : AIBehaviour
		{
				private static readonly string SCRIPT_NAME = typeof(PursuePlayer).Name;

        private static PlayerController PLAYER_MOVEMENT;

        void Awake()
        {
            PLAYER_MOVEMENT = FindObjectOfType<PlayerController>();
        }

				void Start ()
				{
						Initialise ();
				}
		
				public override Vector2 GetForce ()
				{
						var player = GetEntityWithTagName (PLAYER_TAG_NAME, SCRIPT_NAME, LOGGING_ENABLED);

						if (!player)
								return Vector2.zero;

						Vector2 toPlayer = player.transform.position - transform.position;

						float relativeHeading = Vector2.Dot (entity.heading, PLAYER_MOVEMENT.heading);

						if ((Vector2.Dot (toPlayer, entity.heading) > 0)
								&& (relativeHeading < -0.95)) {
								return Arrive (player.transform.position, 10f);
						}

						float LookAheadTime = toPlayer.magnitude
								/ (entity.MaxVelocity + PLAYER_MOVEMENT.velocity.magnitude);

						Vector2 pos = (Vector2)player.transform.position;

						Vector2 seekPos = (pos + (PLAYER_MOVEMENT.velocity * LookAheadTime));

						return Arrive (seekPos, 10f);
				}
		
		
		}
}
