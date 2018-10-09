using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureGauge : MonoBehaviour {

	public GameObject m_dial;
	public float m_dialSpeed = 0.5f;

    private float m_currentPressure = 0.0f;

	void Start()
    {
		m_dial.transform.eulerAngles = new Vector3(0f, 0f, 120f);
	}

	public void UpdatePressure(float p_pressureChange)
    {
        m_currentPressure += p_pressureChange;
		if (m_currentPressure < 0)
            m_currentPressure = 0;
		else if (m_currentPressure > 12)
            m_currentPressure = 12;

		Vector3 dialPosChange = new Vector3(0f, 0f, 20f * p_pressureChange);
		Vector3 newDialPos = Vector3.zero;

		newDialPos = m_dial.transform.eulerAngles - dialPosChange;
		
		StartCoroutine(MoveDial(newDialPos, m_dialSpeed));
	}

	IEnumerator MoveDial(Vector3 p_newPosition, float p_time) {
		float elapsedTime = 0;
		while (elapsedTime < p_time) {
			//dial.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingPos.z, p_newPosition.z, (elapsedTime / p_time)));

			Quaternion currentRotation = m_dial.transform.rotation;
     		Quaternion wantedRotation = Quaternion.Euler(p_newPosition);
     		m_dial.transform.rotation = Quaternion.Lerp(currentRotation, wantedRotation, (elapsedTime / p_time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}
}
