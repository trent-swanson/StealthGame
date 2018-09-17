using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFade : MonoBehaviour {

	private Material standardMat;	
	public Material fadeWall;
	public GameObject[] wallList;

    //For wall with door on it\
    public Material garageDoorMat;
    public Material fadeGarageDoor;
    public GameObject garageDoor;

    public Material smallDoorMat;
    public Material fadeSmallDoor;
    public GameObject smallDoor;


    void Start () {

        wallList = new GameObject[transform.childCount];

		for (int i = 0; i < transform.childCount; i++) {
			wallList[i] = transform.GetChild(i).gameObject;
		}
		standardMat = wallList[0].GetComponent<Renderer>().material;
    }

	public void FadeWall()
    {

        //Fix Later
        if (Time.timeScale == 1)
        {
            for (int i = 0; i < wallList.Length; i++)
            {
                wallList[i].GetComponent<Renderer>().material = fadeWall;
            }

            //If this set of walls has a Door on it
            if (garageDoor != null)
            {
                garageDoor.GetComponent<Renderer>().material = fadeGarageDoor;
            }
            if (smallDoor != null)
            {
                smallDoor.GetComponent<Renderer>().material = fadeSmallDoor;
            }
        }
    }

    public void UnFadeWall()
    {
        for (int i = 0; i < wallList.Length; i++)
        {
            wallList[i].GetComponent<Renderer>().material = standardMat;
        }
        //If this set of walls has a Door on it
        if (garageDoor != null)
        {
            garageDoor.GetComponent<Renderer>().material = garageDoorMat;
        }
        if (smallDoor != null)
        {
            smallDoor.GetComponent<Renderer>().material = smallDoorMat;
        }
    }
}
