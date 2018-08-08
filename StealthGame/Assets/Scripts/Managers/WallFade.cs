using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFade : MonoBehaviour {

	private Material standardMat;
	private Material[] wallList;

	public GameManager.CamDirection camDirection;

	[Space]
	public Material fadeMat;

	void Start () {
		wallList = new Material[transform.childCount];

		standardMat = wallList[0];

		for (int i = 0; i < transform.childCount; i++) {
			wallList[i] = transform.GetChild(i).GetComponent<Material>();
		}

		GameManager.walls.Add(this);
	}

	public void UpdateWallFade() {
		if (GameManager.camDirection == camDirection) {
			FadeWall();
		} else {
			UnfadeWall();
		}
	}

	void FadeWall() {
		for (int i = 0; i < wallList.Length; i++) {
			wallList[i] = fadeMat;
		}
	}

	void UnfadeWall() {
		for (int i = 0; i < wallList.Length; i++) {
			wallList[i] = standardMat;
		}
	}
}
