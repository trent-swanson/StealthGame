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
        m_parentStateMachine.m_currentSelectedNode = null;
        m_parentStateMachine.m_playerController.m_targetInteractable = null;
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

        Queue<NavNode> BFSQueueOpen = new Queue<NavNode>();
        Queue<NavNode> BFSQueueClosed = new Queue<NavNode>();

        BFSQueueOpen.Enqueue(currentNavNode);

        NavNode currentBFSNode = null;

        while (BFSQueueOpen.Count > 0) //BFS implementation
        {
            currentBFSNode = BFSQueueOpen.Dequeue();

            if (!BFSQueueClosed.Contains(currentBFSNode))
            {
                BFSQueueClosed.Enqueue(currentBFSNode);

                foreach (NavNode nextBFSNode in currentBFSNode.m_adjacentNodes)
                {
                    if (!m_parentStateMachine.m_selectableNodes.Contains(nextBFSNode) && !BFSQueueOpen.Contains(nextBFSNode) && !BFSQueueClosed.Contains(nextBFSNode))
                    {
                        int distance = currentBFSNode.m_BFSDistance - 1;

                        if (nextBFSNode.m_BFSDistance < distance && distance >= 0)
                        {
                            nextBFSNode.m_BFSDistance = distance;
                            nextBFSNode.m_BFSPreviousNode = currentBFSNode;

                            if (nextBFSNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || nextBFSNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE)//TODO if we want to move through team mates just compare team values
                            {
                                BFSQueueOpen.Enqueue(nextBFSNode);
                                m_parentStateMachine.m_selectableNodes.Add(nextBFSNode);
                            }
                            else if (nextBFSNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED)
                            {
                                if (nextBFSNode.m_obstructingAgent.m_team != m_playerController.m_team)//enemy on square, only add selectable, not moving through
                                    m_parentStateMachine.m_selectableNodes.Add(nextBFSNode);
                                else if (nextBFSNode.m_obstructingAgent.m_knockedout)// obstructing agent is on same team, hence can select
                                {
                                    m_parentStateMachine.m_selectableNodes.Add(nextBFSNode);

                                    BFSQueueOpen.Enqueue(nextBFSNode);
                                }
                                else
                                {
                                    PlayerController playerController = nextBFSNode.m_obstructingAgent.GetComponent<PlayerController>();
                                    if(playerController!=null && playerController.m_playerStateMachine.m_currentlyHiding)
                                    {
                                        BFSQueueOpen.Enqueue(nextBFSNode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
