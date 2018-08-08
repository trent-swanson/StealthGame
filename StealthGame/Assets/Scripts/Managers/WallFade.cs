using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFade : MonoBehaviour {

	private Material standardMat;
	
	public Material fadeMat;
	public Material[] wallList;

	void Start () {
		wallList = new Material[transform.childCount];

		standardMat = wallList[0];

		for (int i = 0; i < transform.childCount; i++) {
			wallList[i] = transform.GetChild(i).GetComponent<Material>();
		}
        FadeWall();
	}

	public void FadeWall() {
		for (int i = 0; i < wallList.Length; i++) {
			wallList[i] = fadeMat;
		}
	}

	public void UnFadeWall() {
		for (int i = 0; i < wallList.Length; i++) {
			wallList[i] = standardMat;
		}
	}
}
