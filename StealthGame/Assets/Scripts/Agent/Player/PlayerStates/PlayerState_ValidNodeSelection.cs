using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_ValidNodeSelection : PlayerState
{
    private NavNode m_currentNode = null;

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
        m_currentNode = null;
        m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.DRAW_NAVMESH, m_parentStateMachine.m_selectableNodes);
    }

    //-------------------
    //State update, perform any actions for the given state
    //
    //Return bool: Has this state been completed, e.g. Attack has completed, idle would always return true 
    //-------------------
    public override bool UpdateState()
    {
        NavNode newSelectedNavNode = m_parentStateMachine.GetMouseNode();
        if (newSelectedNavNode != null) // Hovering over empty space
        {
            Agent downedAgent = newSelectedNavNode.GetDownedAgent(m_playerController.m_team);

            bool nextNodeUsable = newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || //Node is walkable
                newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE || //Interaction node
                (newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED &&
                newSelectedNavNode.m_obstructingAgent != null &&
                newSelectedNavNode.m_obstructingAgent.m_team != m_playerController.m_team) || //Node contains an enemy 
                downedAgent != null;//Node contains downed ally

            if (m_parentStateMachine.m_selectableNodes.Contains(newSelectedNavNode) && nextNodeUsable)
            {
                if (newSelectedNavNode != m_currentNode)
                {
                    m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.DRAW_PATH, m_parentStateMachine.m_selectableNodes, m_currentNode, newSelectedNavNode, m_parentStateMachine.GetPath(newSelectedNavNode));
                    m_currentNode = newSelectedNavNode;
                    m_parentStateMachine.m_currentSelectedNode = m_currentNode;
                }
            }
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

        return currentSelectedNavNode != null && 
            m_parentStateMachine.m_selectableNodes.Contains(currentSelectedNavNode) && 
            (currentSelectedNavNode.m_obstructingAgent == null || currentSelectedNavNode.m_obstructingAgent.m_team != m_playerController.m_team);
    }
}
