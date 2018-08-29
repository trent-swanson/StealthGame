using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyleSteamGauge : MonoBehaviour {

    private float targetZRot;
    private float oldTargetZRot;
    public GameObject dial;

    public float steamValue = 0;
    private float oldSteamValue = 0;

    public float lerpRate;
    public float shakeMag = 0.6f;
    public float shakeSpeed = 60f;


    void Start () {
        //Ticks up steam over time
        InvokeRepeating("IncreaseSteam", 10, 30);
        ///REPLACE ^^

        //Sets values to equal
        oldTargetZRot = targetZRot;
        oldSteamValue = steamValue = 0;
        TargetValveRot(0);
    }
	
	// Update is called once per frame
	void Update () {
        if (steamValue == oldSteamValue)
        {
            //Constant Shaking of Valve
            ConstantShake();
        }
        else
        {
            //Debug Test
            TargetValveRot(steamValue);
        }

    }

    /// <summary>
    /// Rotation of the Dial on the Steam Gauge, based on Steam Value
    /// </summary>
    /// <param name="steamValue"></param>
    public void TargetValveRot(float steamValue)
    {
        //Min Valve Value
        if (steamValue < 0)
        {
            steamValue = 0;
        }
        //Max Valve Value
        else if (steamValue > 20)
        {
            steamValue = 20;
            oldSteamValue = steamValue;
        }
        targetZRot = -12*(steamValue) + 117.5f;

        //float lerpy = Mathf.Lerp(dial.transform.rotation.z, targetZRot, lerpRate * Time.deltaTime);
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
        if (steamValue > 0)
        {
            dial.transform.rotation = Quaternion.Euler(0, 0, targetZRot - shakeMag * Mathf.Pow(steamValue/3, 0.5f) * Mathf.Sin(shakeSpeed * Time.timeSinceLevelLoad));
        }
        else if (steamValue >= 20)
        {
            dial.transform.rotation = Quaternion.Euler(0, 0, targetZRot - 2 * shakeMag * Mathf.Pow(steamValue / 3, 0.5f) * Mathf.Sin(shakeSpeed * Time.timeSinceLevelLoad));
        }
    }

    public void IncreaseSteam()
    {
        steamValue += 3f;
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        //suspend execution for X seconds
        yield return new WaitForSeconds(time);
        //Changes oldTargetRot
        oldTargetZRot = targetZRot;
    }

}
