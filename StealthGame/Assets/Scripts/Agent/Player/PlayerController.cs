using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Agent {

    [Space]
    [Space]
    public Sprite portrait;

    static CameraController m_cameraController = null;

    private PlayerUI m_playerUI = null;
    private PlayerActions m_playerActions = null;
    public AnimationDetection[] m_animationDetection = null;


    protected override void Start()
    {
        base.Start();
        m_cameraController = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();

        m_playerUI = GetComponent<PlayerUI>();
        m_playerActions = GetComponent<PlayerActions>();
        AnimationDetection[] m_animationDetection = m_animator.GetBehaviours<AnimationDetection>();

        foreach (AnimationDetection animationDetection in m_animationDetection)
        {
            animationDetection.m_playerController = this;
        }
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
        m_playerActions.InitActions();
        m_playerUI.StartUI();
        m_cameraController.Focus(transform);
    }

    //Constant update while agent is selected
    public override void AgentTurnUpdate()
    {
        m_playerActions.UpdateActions();
        m_playerUI.UpdateUI();
        if(m_currentActionPoints <= 0)
        {
            m_turnManager.EndUnitTurn(this);
        }
    }

    //Runs when agent is removed from team list, end of turn
    public override void AgentTurnEnd()
    {
        base.AgentTurnEnd();
        m_playerActions.ActionEnd();
    }

    public override bool IsMoving()
    {
        return (m_playerActions.m_currentActionState == PlayerActions.ACTION_STATE.ACTION_PERFORM);
    }

    public void EndAnimationState()
    {
        m_playerActions.AnimationFinished();
    }
}
