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
        FACING_DIR m_largestThreatDir = GetLargestThreatDir();

        if (m_largestThreatDir != FACING_DIR.NONE && m_parentStateMachine.m_currentSelectedNode.m_abilityToWallHide[(int)m_largestThreatDir]) //Can hide in largest threat direction
        {
            m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide(m_largestThreatDir, m_playerController));
        }
        else //Pick random wall to hide on 
        {
            //TODO get next closest wall to hide on
            if (m_parentStateMachine.m_currentSelectedNode.m_abilityToWallHide[0])
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)0, m_playerController));
            else if (m_parentStateMachine.m_currentSelectedNode.m_abilityToWallHide[1])
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)1, m_playerController));
            else if (m_parentStateMachine.m_currentSelectedNode.m_abilityToWallHide[2])
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)2, m_playerController));
            else if (m_parentStateMachine.m_currentSelectedNode.m_abilityToWallHide[3])
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)3, m_playerController));
        }

        m_parentStateMachine.m_currentlyHiding = true;
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
        return true;
    }

    public FACING_DIR GetLargestThreatDir()
    {
        //Get closest guard
        List<Agent> m_guards = m_playerController.m_turnManager.m_NPCTeam;

        float closestDistance = Mathf.Infinity;
        Agent closestGuard = null;
        foreach (Agent guard in m_guards)
        {
            float distance = Vector3.Distance(transform.position, guard.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGuard = guard;
            }
        }

        if (closestGuard != null)
        {
            return Agent.GetFacingDir(closestGuard.transform.position - transform.position);
        }
        return FACING_DIR.NONE;
    }
}
