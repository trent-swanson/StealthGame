using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureGauge : MonoBehaviour
{
    public GameObject m_dial = null;

    public float m_fluctuateAmount = 0.2f;
    public float m_fluctuateErraticness = 0.2f;
    public float m_minAngle = 120.0f;
    public float m_maxAngle = -120.0f;

    [SerializeField]
    private float m_currentPressure = 0.0f;

    private void Start()
    {
        m_currentPressure = m_minAngle;

        m_dial.transform.rotation = Quaternion.Euler(0, 0, m_minAngle);
    }

    // Update is called once per frame
    void Update ()
    {
        m_dial.transform.rotation = Quaternion.Euler(0, 0, m_currentPressure + m_fluctuateAmount * Mathf.Sin(Time.realtimeSinceStartup * m_fluctuateErraticness));
    }

    public void IncreaseDialAngle(float degrees)
    {
        m_currentPressure -= degrees;
        m_currentPressure = Mathf.Clamp(m_currentPressure, m_maxAngle, m_minAngle);
    }
}
