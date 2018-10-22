using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_InputStart : PlayerState
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
        GetAllSelectableNodes();
        m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.DRAW_NAVMESH, m_parentStateMachine.m_selectableNodes);
        m_playerUI.UpdateUI();
    }

    //-------------------
    //State update, perform any actions for the given state
    //
    //Return bool: Has this state been completed, e.g. Attack has completed, idle would always return true 
    //-------------------
    public override bool UpdateState()
    {
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
        return true;
    }

    private void GetAllSelectableNodes()
    {
        m_parentStateMachine.m_selectableNodes.Clear();

        NavNode currentNavNode = m_playerController.m_currentNavNode;
        if (currentNavNode == null)
            return;

        currentNavNode.m_BFSDistance = m_playerController.m_currentActionPoints;
        currentNavNode.m_BFSPreviousNode = null;

        Queue<NavNode> BFSQueue = new Queue<NavNode>();
        BFSQueue.Enqueue(currentNavNode);
        NavNode currentBFSNode = null;

        while (BFSQueue.Count > 0) //BFS implementation
        {
            currentBFSNode = BFSQueue.Dequeue();
            m_parentStateMachine.m_selectableNodes.Add(currentBFSNode);

            foreach (NavNode nextBFSNode in currentBFSNode.m_adjacentNodes)
            {
                if (!m_parentStateMachine.m_selectableNodes.Contains(nextBFSNode) && !BFSQueue.Contains(nextBFSNode))
                {
                    int distance = currentBFSNode.m_BFSDistance - 1;

                    nextBFSNode.m_BFSDistance = distance;
                    nextBFSNode.m_BFSPreviousNode = currentBFSNode;

                    if (distance >= 0)
                    {
                        if (nextBFSNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE)//TODO if we want to move through team mates just compare team values
                            BFSQueue.Enqueue(nextBFSNode);
                        else if (nextBFSNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || nextBFSNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE)
                            m_parentStateMachine.m_selectableNodes.Add(nextBFSNode);
                    }
                }
            }
        }
    }
}
