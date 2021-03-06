﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Agent
{
    [Space]
    public Sprite portrait;

    public PlayerUI m_playerUI = null;
    public PlayerStateMachine m_playerStateMachine = null;

    public GameObject m_goldBag = null;

    //--------------------------------------------------------------------------------------
    // Initialisation
    //--------------------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        m_playerUI = GetComponent<PlayerUI>();
        m_playerStateMachine = GetComponent<PlayerStateMachine>();

        if (m_goldBag != null)
            m_goldBag.SetActive(false);
    }

    //Start of turn, only runs once per turn
    public override void AgentTurnInit()
    {
        base.AgentTurnInit();
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public override void AgentSelected()
    {
        m_playerStateMachine.TurnStartStateMachine();
        m_playerUI.StartUI();
        m_cameraController.Focus(transform);
    }

    //Constant update while agent is selected
    public override AGENT_UPDATE_STATE AgentTurnUpdate()
    {
        m_playerStateMachine.UpdateStateMachine();

        if (!m_agentAnimationController.m_playNextAnimation || m_agentAnimationController.m_animationSteps.Count != 0)
            return AGENT_UPDATE_STATE.PERFORMING_ACTIONS;

        return AGENT_UPDATE_STATE.AWAITING_INPUT;
    }

    //Runs when agent is removed from team list, end of turn
    public override void AgentTurnEnd()
    {
        EndTurn();
        m_playerStateMachine.TurnEndStateMachine();
        m_playerUI.UpdateUI();
        m_agentInventoryUI.DisableInventory();

        //Reset all nav node BFS vals to -1
        foreach (NavNode navNode in m_playerStateMachine.m_selectableNodes)
        {
            navNode.m_BFSDistance = -1;
        }
        m_currentNavNode.m_BFSDistance = -1;
    }

    public override void Knockout()
    {
        base.Knockout();
        StartCoroutine(KnockoutDelay());
    }

    //Wall hiding for end of turn
    public void EndTurn()
    {
        if (m_playerStateMachine.m_currentlyHiding || m_knockedout || m_interaction == INTERACTION_TYPE.INTERACTABLE)
            return;

        FACING_DIR m_largestThreatDir = GetLargestThreatDir();

        if (m_largestThreatDir != FACING_DIR.NONE && m_currentNavNode.m_abilityToWallHide[(int)m_largestThreatDir]) //Can hide in largest threat direction
        {
            m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide(m_largestThreatDir, this));

            m_playerStateMachine.m_currentlyHiding = true;
        }
        else //Pick random wall to hide on 
        {
            //TODO get next closest wall to hide on
            if (m_currentNavNode.m_abilityToWallHide[0])
            {
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)0, this));

                m_playerStateMachine.m_currentlyHiding = true;
            }
            else if (m_currentNavNode.m_abilityToWallHide[1])
            {
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)1, this));

                m_playerStateMachine.m_currentlyHiding = true;
            }
            else if (m_currentNavNode.m_abilityToWallHide[2])
            {
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)2, this));

                m_playerStateMachine.m_currentlyHiding = true;
            }
            else if (m_currentNavNode.m_abilityToWallHide[3])
            {
                m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.EndTurnWallHide((FACING_DIR)3, this));

                m_playerStateMachine.m_currentlyHiding = true;
            }
        }

        m_agentAnimationController.PlayNextAnimation();
    }

    private FACING_DIR GetLargestThreatDir()
    {
        //Get closest guard
        List<Agent> m_guards = m_NPCTurn.m_team;

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

    public void AddGoldBag()
    {
        if (m_goldBag != null)
            m_goldBag.SetActive(true);
    }

    //Used to delay camera movement on player death
    public IEnumerator KnockoutDelay()
    {
        yield return new WaitForSeconds(m_playerTurn.m_showDeathDelay);

        m_playerTurn.NextPlayer();
    }
}
