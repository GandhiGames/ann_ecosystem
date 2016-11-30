using UnityEngine;
using System.Collections;

namespace Automation
{
    public interface ANNAnimationController
    {

    }

    [RequireComponent(typeof(Animator), typeof(MovingAgent))]
    public class ANNAnimationControllerImpl : MonoBehaviour, ANNAnimationController
    {
        private static readonly int XVELOCITY_HASH = Animator.StringToHash("XVelocity");
        private static readonly int YVELOCITY_HASH = Animator.StringToHash("YVelocity");
        private static readonly int VELOCITY_HASH = Animator.StringToHash("VelocityNorm");

        private Animator m_Animator;
        private MovingAgent m_Entity;

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Entity = GetComponent<MovingAgent>();
        }

        void Update()
        {
            m_Animator.SetFloat(XVELOCITY_HASH, m_Entity.heading.x);
            m_Animator.SetFloat(YVELOCITY_HASH, m_Entity.heading.y);
            m_Animator.SetFloat(VELOCITY_HASH, m_Entity.velocity.magnitude);
        }
    }
}