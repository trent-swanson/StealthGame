using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Agent : MonoBehaviour {

    protected SquadManager squadManager;

    [Header("DebugDebugging Only")]
    [Tooltip("Do Not Assign")]
    public bool m_turn = false;
    [Tooltip("Do Not Assign")]
    public bool m_knockedout = false;

    [Tooltip("# of actions unit can perform")]
    public int m_maxActionPoints = 2;
    public int m_currentActionPoints = 2;
    [Tooltip("Speed of an agent")]
    public float m_moveSpeed = 1.0f;

    [Space]
    public float m_highlightInteractablesRange = 6;

    protected TurnManager m_turnManager = null;
    public NavNode m_currentNavNode = null;

    public TurnManager.TEAM m_team = TurnManager.TEAM.AI;

    [Space]
    public List<Item> m_currentItems = new List<Item>();

    protected virtual void Start()
    {
        //New Stuff
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("NavNode")))
            m_currentNavNode = hit.collider.GetComponent<NavNode>();

        if (m_currentNavNode != null) // Current node isobstructed, that is taken up by player
            m_currentNavNode.m_nodeState = NavNode.NODE_STATE.OBSTRUCTED;

        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();

    }

    //Start of turn, only runs once per turn
    public virtual void AgentTurnInit()
    {
        m_currentActionPoints = m_maxActionPoints;
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public virtual void AgentSelected() { }

    //Constant update while agent is selected
    public virtual void AgentTurnUpdate() { }

    //Runs when agent is removed from team list, end of turn
    public virtual void AgentTurnEnd()
    {
        m_turnManager.EndUnitTurn(this);
    }

	public void Knockout() {
		m_knockedout = true;
		transform.position = new Vector3(0, 100, 0);
		AgentTurnEnd();
	}
}
