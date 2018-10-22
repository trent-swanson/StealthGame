using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_InvalidNodeSelection : PlayerState
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
        m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.REMOVE_PATH, m_parentStateMachine.m_selectableNodes, m_parentStateMachine.m_currentSelectedNode);
    }

    //-------------------
    //State update, perform any actions for the given state
    //
    //Return bool: Has this state been completed, e.g. Attack has completed, idle would always return true 
    //-------------------
    public override bool UpdateState()
    {
        NavNode newSelectedNavNode = m_parentStateMachine.GetMouseNode();
        if (newSelectedNavNode != null && m_parentStateMachine.m_selectableNodes.Contains(newSelectedNavNode))
        {
            m_parentStateMachine.m_currentSelectedNode = newSelectedNavNode;
        }
        return true;
    }

    //-------------------
    //When swapping to a new state, this is called.
    //-------------------
    public override void StateEnd()
    {

    }

    //-------------------
    //Do all of this states preconditions return true
    //
    //Return bool: Is this valid, e.g. Death requires players to have no health
    //-------------------
    public override bool IsValid()
    {
        NavNode currentSelectedNavNode = m_parentStateMachine.GetMouseNode();

        return currentSelectedNavNode == null || !m_parentStateMachine.m_selectableNodes.Contains(currentSelectedNavNode);
    }
}
