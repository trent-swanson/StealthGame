using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_InputEnd : PlayerState
{
    //-------------------
    //Initilse the state
    //-------------------
    public override void StateInit()
    {
        base.StateInit();
    }

    //-------------------
    //When swapping to this state, this is called.
    //-------------------
    public override void StateStart()
    {
    }

    //-------------------
    //State update, perform any actions for the given state
    //
    //Return bool: Has this state been completed, e.g. Attack has completed, idle would always return true 
    //-------------------
    public override bool UpdateState()
    {

        if (m_agentAnimationController.m_animationSteps.Count == 0)//End of move
        {
            m_playerController.ChangeCurrentNavNode(m_playerController.m_path[m_playerController.m_path.Count - 1]);
            return true;
        }

        return false;
    }

    //-------------------
    //When swapping to a new state, this is called.
    //-------------------
    public override void StateEnd()
    {
        if(m_playerController.m_currentActionPoints == 0)
        {
            m_playerController.m_playerTurn.AutoEndTurn();
        }

        m_parentStateMachine.m_selectableNodes.Clear();
        m_parentStateMachine.m_currentSelectedNode = null;

    }

    //-------------------
    //Do all of this states preconditions return true
    //
    //Return bool: Is this valid, e.g. Death requires players to have no health
    //-------------------
    public override bool IsValid()
    {
        return true;
    }
}
