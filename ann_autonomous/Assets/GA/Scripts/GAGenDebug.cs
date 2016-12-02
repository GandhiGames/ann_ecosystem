using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Automation
{
    public class GAGenDebug : MonoBehaviour
    {
        public GASimulation simulation;

        private Text m_Text;

        void Awake()
        {
            m_Text = GetComponent<Text>();
        }

        void Update()
        {
            m_Text.text = "Pred Gen: " + simulation.algorithms[0].generationNumber + '\n' +
                "Prey Gen: " + simulation.algorithms[1].generationNumber;
        }
    }
}