using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Automation
{
    public interface Sight
    {
        List<MovingAgent> GetAgentsInSightWithTag(string tag);
        void Clear();
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class AgentSight : MonoBehaviour, Sight
    {
        public float sightRadius = 20f;

        public string[] tagNames;

        private readonly static List<MovingAgent> EMPTY_LIST = new List<MovingAgent>();

        private Dictionary<string, List<MovingAgent>> m_AgentsInSight = new Dictionary<string, List<MovingAgent>>();

        private int m_ID;

        //public List<MovingAgent> vegetationInSight { get; private set; }

        void Awake()
        {
            m_ID = transform.parent.gameObject.GetInstanceID();
        }

        void Start()
        {
            var collider = GetComponent<CircleCollider2D>();
            collider.radius = sightRadius;
            collider.isTrigger = true;

            foreach(var tag in tagNames)
            {
                m_AgentsInSight.Add(tag, new List<MovingAgent>());
            }
        }

        public List<MovingAgent> GetAgentsInSightWithTag(string tag)
        {
            if(m_AgentsInSight.ContainsKey(tag))
            {
                return m_AgentsInSight[tag];
            }

            return EMPTY_LIST;
        }

        public void Clear()
        {
            foreach (var tag in tagNames)
            {
                m_AgentsInSight[tag].Clear();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(!m_AgentsInSight.ContainsKey(other.gameObject.tag)
                || other.gameObject.GetInstanceID().Equals(m_ID))
            {
                return;
            }

            m_AgentsInSight[other.gameObject.tag].Add(other.GetComponent<MovingAgent>());

           
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (!m_AgentsInSight.ContainsKey(other.gameObject.tag)
                || other.gameObject.GetInstanceID().Equals(m_ID))
            {
                return;
            }

            m_AgentsInSight[other.gameObject.tag].Remove(other.GetComponent<MovingAgent>());

        }
    }
}