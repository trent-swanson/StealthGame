using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turnmanageroverride : MonoBehaviour
{
    TurnManager m_turnManager = null;

    public PlayerController m_playerController = null;
    public NPC m_enemyController = null;

    // Use this for initialization
    void Start ()
    {
        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_turnManager.enabled = false;
            m_playerController.GetComponent<Animator>().SetTrigger("Punch");
            m_enemyController.GetComponent<Animator>().SetTrigger("Death");
        }
	}
}
