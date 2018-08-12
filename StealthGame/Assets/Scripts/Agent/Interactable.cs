using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Animation m_animation;

    [Tooltip("Does this interactable require the player to stop moving?")]
    public bool m_requiresIdle = false;

    public enum INTERACTABLE_TYPE {WALL_HIDE, ITEM, ATTACK}
    public INTERACTABLE_TYPE m_interactableType = INTERACTABLE_TYPE.WALL_HIDE;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
