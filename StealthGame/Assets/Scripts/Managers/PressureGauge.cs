using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureGauge : MonoBehaviour {

	public GameObject dial;
	public float dialSpeed = 0.5f;

	void Start() {
		GameManager.Pressure = 0;
		dial.transform.eulerAngles = new Vector3(0f, 0f, 120f);
	}

	void Update() {
		//For Debug Only
		if(Input.GetKeyUp(KeyCode.K)) {
			UpdatePressure(1);
		}
		if(Input.GetKeyUp(KeyCode.L)) {
			UpdatePressure(-1);
		}
	}

	public void UpdatePressure(int p_pressureChange) {
		int savedPressure = GameManager.Pressure;

		GameManager.Pressure += p_pressureChange;
		if (GameManager.Pressure < 0)
			GameManager.Pressure = 0;
		else if (GameManager.Pressure > 12)
			GameManager.Pressure = 12;
		
		Debug.Log(GameManager.Pressure);

		Vector3 dialPosChange = new Vector3(0f, 0f, 20f * p_pressureChange);
		Vector3 newDialPos = Vector3.zero;

		newDialPos = dial.transform.eulerAngles - dialPosChange;

		if (GameManager.Pressure <= 0) {
			newDialPos = new Vector3(0f,0f,120f);
			Debug.Log("120" + newDialPos);
		}
		else if (GameManager.Pressure >= 12) {
			newDialPos = new Vector3(0f,0f,-120f);
			Debug.Log("-120" + newDialPos);
		}
		
		StartCoroutine(MoveDial(newDialPos, dialSpeed));
	}

	IEnumerator MoveDial(Vector3 p_newPosition, float p_time) {
		float elapsedTime = 0;
		while (elapsedTime < p_time) {
			//dial.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingPos.z, p_newPosition.z, (elapsedTime / p_time)));

			Quaternion currentRotation = dial.transform.rotation;
     		Quaternion wantedRotation = Quaternion.Euler(p_newPosition);
     		dial.transform.rotation = Quaternion.Lerp(currentRotation, wantedRotation, (elapsedTime / p_time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}
}
