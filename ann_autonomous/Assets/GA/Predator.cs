using UnityEngine;
using System;
using System.Collections.Generic;

namespace Automation
{

    public interface GAAgent
    {
        Vector2 position { get; }
        bool isAlive { get; }
        int timeAlive { get; }
        bool isAddedToPool { get; set; }
    }

    public class Predator : MonoBehaviour, GAAgent, IComparable
    {
        public int maxEnergy;
        public float sightRadius;
        public float lateralRadius;

        public Vector2 position { get { return transform.position; } }
        public Vector2 velocity { get; private set; }
        public Vector2 heading { get; private set; }
        public bool isAlive { get { return m_currentEnergy > 0; } }
        public NeuralNet neuralNet { get; set; }
        public int timeAlive { get; private set; }
        public bool isAddedToPool { get; set; }

        private static GeneticAlgorithm m_GA;

        private BehaviourManager m_BehaviourManager;
        private int m_currentEnergy;
        private List<GAAgent> m_PreyInSight = new List<GAAgent>();
        private List<GAAgent> m_PredInSight = new List<GAAgent>();

        void Awake()
        {
            if (m_GA == null)
            {
                m_GA = FindObjectOfType<GeneticAlgorithm>();
            }

            m_BehaviourManager = GetComponent<BehaviourManager>();
        }

        public void Reset()
        {
            m_BehaviourManager.Reset();
            m_currentEnergy = maxEnergy;
            timeAlive = 0;

            velocity = Vector2.zero;

            m_PreyInSight.Clear();
            m_PredInSight.Clear();

            isAddedToPool = false;
        }

        public void Mutate()
        {
            List<float> weights = new List<float>();
            weights.AddRange(neuralNet.GetWeights());

            // int mutate = (int)Utilities.RandomMinMax(0, weights.Count);
            // weights[mutate] += (Utilities.RandomMinMax(-1, 1) * Utilities.MaxPerturbation);

            for (int i = 0; i < weights.Count; ++i)
            {
                if (Utilities.instance.RandomMinMax(0, 1) < m_GA.mutationRate)
                {
                    weights[i] += (Utilities.instance.RandomMinMax(-1, 1) * m_GA.maxPerturbation);
                }
            }

            neuralNet.SetWeights(ref weights);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherAgent = obj as GAAgent;

            if (otherAgent != null)
            {
                return otherAgent.timeAlive - this.timeAlive;
            }
            else
            {
                return 1;
            }
        }

        public List<float> GetNetworkWeights()
        {
            return neuralNet.GetWeights();
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

        private float CheckPreyInSight(List<GAAgent> agents)
        {
            float numOfAgentsInSight = 0;
            for (int i = 0; i < agents.Count; i++)
            {
                if (!this.Equals(agents[i]) && agents[i].isAlive)
                {
                    Vector2 to = agents[i].position - (Vector2)transform.position;

                    if (to.sqrMagnitude < (sightRadius * sightRadius))
                    {
                        m_PreyInSight.Add(agents[i]);
                        numOfAgentsInSight += 1f / m_GA.maxPrey;
                    }
                }
            }

            return numOfAgentsInSight;
        }
    }
}