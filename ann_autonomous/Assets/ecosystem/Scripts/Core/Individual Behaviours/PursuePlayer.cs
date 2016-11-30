using UnityEngine;
using System.Collections;

namespace Automation
{
		/// <summary>
		/// Attempts to intercept player, taking into account the players heading. 
		/// </summary>
		public class PursuePlayer : AIBehaviour
		{
        public string toPursueTagName;

   

                private static readonly string SCRIPT_NAME = typeof(PursuePlayer).Name;

        private static PlayerController PLAYER_MOVEMENT;

        protected override void Awake()
        {
            base.Awake();

            if (PLAYER_MOVEMENT == null)
            {
                PLAYER_MOVEMENT = FindObjectOfType<PlayerController>();
            }
        }

        void Start()
        {
            if (toPursueTagName.Equals(""))
            {
                Debug.LogWarning("No tag name set");
            }
        }

        public override Vector2 GetForce ()
				{
						var player = GetEntityWithTagName (toPursueTagName, SCRIPT_NAME, LOGGING_ENABLED);

						if (!player)
								return Vector2.zero;

						Vector2 toPlayer = player.transform.position - transform.position;

						float relativeHeading = Vector2.Dot (m_Entity.heading, PLAYER_MOVEMENT.heading);

						if ((Vector2.Dot (toPlayer, m_Entity.heading) > 0)
								&& (relativeHeading < -0.95)) {
								return Arrive (player.transform.position, 10f);
						}

						float LookAheadTime = toPlayer.magnitude
								/ (m_Entity.maxVelocity + PLAYER_MOVEMENT.velocity.magnitude);

						Vector2 pos = (Vector2)player.transform.position;

						Vector2 seekPos = (pos + (PLAYER_MOVEMENT.velocity * LookAheadTime));

						return Arrive (seekPos, 10f);
				}
		
		
		}
}
