using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyleSteamGauge : MonoBehaviour {

    private GameObject dial;
    private float targetZRot;
    private float oldTargetZRot;

    public float steamValue;
    private float oldSteamValue;

    public float lerpRate;
    public float shakeMag = 0.6f;
    public float shakeSpeed = 60f;

	// Use this for initialization
	void Start () {
        dial = transform.GetChild(0).gameObject;
        //dial.transform.rotation = Quaternion.Euler(0, 0, 0);

        oldTargetZRot = targetZRot;
        oldSteamValue = steamValue;
	}
	
	// Update is called once per frame
	void Update () {
        if (steamValue == oldSteamValue)
        {
            //Constant Shaking of Valve
            ConstantShake();
            Debug.Log("Shaking");
        }
        else
        {
            //Debug Test
            TargetValveRot(steamValue);
            Debug.Log("Moving");
        }

        //TestMove();

    }

    void TestMove()
    {
        //Min Valve Value
        if (steamValue < 5)
        {
            steamValue = 0;
        }
        //Max Valve Value
        else if (steamValue > 35)
        {
            steamValue = 35;
        }
        targetZRot = -9 * steamValue;
        dial.transform.rotation = Quaternion.Euler(0, 0, targetZRot);

        //Constant Drop
        dial.transform.rotation = Quaternion.Euler(0, 0, dial.transform.rotation.z - shakeMag * Mathf.Abs(Mathf.Sin(shakeSpeed * Time.timeSinceLevelLoad)));
    }


    /// <summary>
    /// Rotation of the Dial on the Steam Gauge, based on Steam Value
    /// </summary>
    /// <param name="steamValue"></param>
    public void TargetValveRot(float steamValue)
    {
        //Min Valve Value
        if (steamValue < 5)
        {
            steamValue = 0;
        }
        //Max Valve Value
        else if (steamValue > 35)
        {
            steamValue = 35;
            oldSteamValue = steamValue;
        }
        targetZRot = -9*(steamValue) + 45;

        float lerpy = Mathf.Lerp(dial.transform.rotation.z, targetZRot, lerpRate * Time.deltaTime);
        dial.transform.rotation = Quaternion.Euler(0, 0, targetZRot);

        //Changes to shake check
        if (Mathf.Abs(dial.transform.rotation.z - targetZRot) <= 2f)
        {
            oldSteamValue = steamValue;
        }
        //DELETE ME
        oldSteamValue = steamValue;

    }

    private void MoveDial()
    {
        dial.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, dial.transform.rotation.z), Quaternion.Euler(0,0,targetZRot), lerpRate * Time.deltaTime);
        //Updates oldTargetZRot to = targetZRot
        StartCoroutine(ExecuteAfterTime(2));
    }

    private void ConstantShake()
    {
        if (steamValue > 5)
        {
            dial.transform.rotation = Quaternion.Euler(0, 0, targetZRot - shakeMag * Mathf.Sin(shakeSpeed * Time.timeSinceLevelLoad));
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        //suspend execution for X seconds
        yield return new WaitForSeconds(time);
        //Changes oldTargetRot
        oldTargetZRot = targetZRot;
    }

}
