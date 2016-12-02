using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GATimeScaler : MonoBehaviour
{
    private Slider m_Slider;

    void Awake()
    {
        m_Slider = GetComponent<Slider>();
    }

    private void Update()
    {
        Time.timeScale = m_Slider.value;
    }
}
