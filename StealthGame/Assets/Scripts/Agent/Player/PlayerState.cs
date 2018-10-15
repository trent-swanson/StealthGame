using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public List<PlayerState> m_nextStates = new List<PlayerState>();
    public PlayerState m_defaultState = null;

    protected PlayerUI m_playerUI = null;
    protected AgentAnimationController m_agentAnimationController = null;
    protected PlayerController m_playerController = null;

    protected PlayerStateMachine m_parentStateMachine = null;

    //-------------------
    //Initilse the state
    //-------------------
    protected virtual void Start()
    {
        m_playerUI = GetComponent<PlayerUI>();
        m_agentAnimationController = GetComponent<AgentAnimationController>();
        m_playerController = GetComponent<PlayerController>();
        m_parentStateMachine = GetComponent<PlayerStateMachine>();
    }

    //-------------------
    //When swapping to this state, this is called.
    //-------------------
    public virtual void StateStart()
    {

    }

    //-------------------
    //State update, perform any actions for the given state
    //
    //Return bool: Has this state been completed, e.g. Attack has completed, idle would always return true 
    //-------------------
    public virtual bool UpdateState()
    {
        return true;
    }

    //-------------------
    //When swapping to a new state, this is called.
    //-------------------
    public virtual void StateEnd()
    {

    }

    //-------------------
    //Do all of this states preconditions return true
    //
    //Return bool: Is this valid, e.g. Death requires players to have no health
    //-------------------
    public virtual bool IsValid()
    {
        return false;
    }
}
