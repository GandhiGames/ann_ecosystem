﻿using UnityEngine;
using System.Collections;

namespace Automation
{
    public class Wander : AIBehaviour
    {
        // The amount of random displacement. Lower this number of the character is too "jittery".
        public float Jitter = 50f;

        public float WanderRadius = 3f;
        public float WanderDistance = 10f;

        private Vector2 wanderTarget;

        void Start()
        {
            float theta = (float)Random.Range(0f, 1f) * (float)(Mathf.PI * 2);

            wanderTarget = new Vector2(WanderRadius * Mathf.Cos(theta),
                           WanderRadius * Mathf.Sin(theta));
        }

        public override Vector2 GetForce()
        {

            float jitter = Jitter * Time.deltaTime;

            wanderTarget += new Vector2(Random.Range(-jitter, jitter), Random.Range(-jitter, jitter));

            wanderTarget.Normalize();

            wanderTarget *= WanderRadius;


            //Vector2 target = wanderTarget + new Vector2(WanderDistance, 0);

            //Vector2 targetPosition = transform.TransformPoint (target);

            Vector2 displacement = m_Entity.heading.normalized * WanderDistance;

            Vector2 targetPosition = (Vector2)transform.position + wanderTarget + displacement;

            return targetPosition - (Vector2)transform.position;
        }


    }
}
