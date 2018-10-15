using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Agent {

    [Space]
    [Space]
    public Sprite portrait;

    static CameraController m_cameraController = null;

    public PlayerUI m_playerUI = null;
    private PlayerStateMachine m_playerStateMachine = null;

    protected override void Start()
    {
        base.Start();
        m_cameraController = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();

        m_playerUI = GetComponent<PlayerUI>();
        m_playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    //Start of turn, only runs once per turn
    public override void AgentTurnInit()
    {
        base.AgentTurnInit();

        m_playerUI.InitUI();
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

        if(!m_agentAnimationController.m_playNextAnimation || m_agentAnimationController.m_animationSteps.Count !=0)
            return AGENT_UPDATE_STATE.PERFORMING_ACTIONS;

        if (m_currentActionPoints <= 0)
            return AGENT_UPDATE_STATE.END_TURN;

        return AGENT_UPDATE_STATE.AWAITING_INPUT;
    }

    //Runs when agent is removed from team list, end of turn
    public override void AgentTurnEnd()
    {
        base.AgentTurnEnd();
        m_playerStateMachine.TurnEndStateMachine();
        m_playerUI.UpdateUI();
    }
}
