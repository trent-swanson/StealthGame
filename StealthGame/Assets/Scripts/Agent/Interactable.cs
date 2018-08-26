using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Animation m_animation;

    [Tooltip("Does this interactable require the player to stop moving?")]
    public bool m_requiresIdle = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
