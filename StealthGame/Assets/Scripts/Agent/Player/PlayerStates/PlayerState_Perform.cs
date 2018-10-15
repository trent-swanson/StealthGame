using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Perform : PlayerState
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
        m_playerController.m_currentActionPoints = m_parentStateMachine.m_currentSelectedNode.m_BFSDistance;//Set action points to node value

        //Get animation steps
        m_agentAnimationController.m_animationSteps.Clear();

        if (m_parentStateMachine.m_currentlyHiding)//Previously hiding on wall, so defualt by adding idle
        {
            m_agentAnimationController.m_animationSteps.Add(AnimationManager.ANIMATION_STEP.IDLE);
            m_parentStateMachine.m_currentlyHiding = false;
        }

        m_playerController.m_interaction = INTERACTION_TYPE.NONE;//Reset interaction

        if (m_parentStateMachine.m_currentSelectedNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED)//Attacking as were moving to a obstructed tile
        {
            m_playerController.m_targetAgent = m_parentStateMachine.m_currentSelectedNode.m_obstructingAgent;

            if (m_agentAnimationController.m_animationSteps.Count == 1)//was previously on a wall, in this case want to setup wall attack it attack is nearby
            {
                m_playerController.m_path = m_parentStateMachine.GetPath(m_parentStateMachine.m_currentSelectedNode);

                if (m_playerController.m_path.Count == 2)//Attakcing adjacent tile
                {
                    m_playerController.m_interaction = INTERACTION_TYPE.WALL_ATTACK;
                    m_agentAnimationController.m_animationSteps.Clear();
                }
                else
                {
                    m_playerController.m_interaction = INTERACTION_TYPE.ATTACK;
                }

                m_playerController.m_path.RemoveAt(m_playerController.m_path.Count - 1);
            }
            else
            {
                m_playerController.m_interaction = INTERACTION_TYPE.ATTACK;

                m_playerController.m_path = m_parentStateMachine.GetPath(m_parentStateMachine.m_currentSelectedNode);
                m_playerController.m_path.RemoveAt(m_playerController.m_path.Count - 1); //As were attackig no need to move to last tile
            }
        }
        else if (m_parentStateMachine.m_currentSelectedNode.m_item != null)//Picking up item
        {
            m_playerController.m_interaction = INTERACTION_TYPE.PICKUP_ITEM;
            m_playerController.m_targetItem = m_parentStateMachine.m_currentSelectedNode.m_item;

            m_playerController.m_path = m_parentStateMachine.GetPath(m_parentStateMachine.m_currentSelectedNode);
        }
        else if (m_parentStateMachine.m_currentSelectedNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE)//Using interactable
        {
            m_playerController.m_interaction = INTERACTION_TYPE.INTERACTABLE;
            m_playerController.m_targetInteractable = m_parentStateMachine.m_currentSelectedNode.m_interactable;

            //Get new path to interactable node
            m_playerController.m_path = m_parentStateMachine.GetPath(m_parentStateMachine.m_currentSelectedNode);
        }
        else if (m_parentStateMachine.m_currentSelectedNode.GetDownedAgent(m_playerController.m_team) != null)//Reviving team mate
        {
            m_playerController.m_interaction = INTERACTION_TYPE.REVIVE;
            m_playerController.m_targetAgent = m_parentStateMachine.m_currentSelectedNode.GetDownedAgent(m_playerController.m_team);

            m_playerController.m_path = m_parentStateMachine.GetPath(m_parentStateMachine.m_currentSelectedNode);
            m_playerController.m_path.RemoveAt(m_playerController.m_path.Count - 1); //As were reviving no need to move to last tile
        }
        else //Default to just walk to node
        {
            m_playerController.m_path = m_parentStateMachine.GetPath(m_parentStateMachine.m_currentSelectedNode);
        }

        transform.position = m_playerController.m_path[0].m_nodeTop;//Move to top of node to remove any minor offsets due to float errors

        m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.GetPlayerAnimationSteps(this.m_playerController, m_playerController.m_path, m_playerController.m_interaction));

        m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.REMOVE_NAVMESH, m_parentStateMachine.m_selectableNodes, m_parentStateMachine.m_currentSelectedNode);//Remove UI
    }

    //-------------------
    //State update, perform any actions for the given state
    //
    //Return bool: Has this state been completed, e.g. Attack has completed, idle would always return true 
    //-------------------
    public override bool UpdateState()
    {
        if (m_agentAnimationController.m_playNextAnimation)//End of animation
        {
            if (m_agentAnimationController.m_animationSteps.Count == 0)//End of move
            {
                m_playerController.ChangeCurrentNavNode(m_playerController.m_path[m_playerController.m_path.Count - 1]);
                return true;
            }

            m_agentAnimationController.PlayNextAnimation();
            m_playerUI.UpdateUI();
        }

        return false;
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
        return Input.GetMouseButtonDown(0) && m_parentStateMachine.m_selectableNodes.Contains(m_parentStateMachine.m_currentSelectedNode);
    }
}
