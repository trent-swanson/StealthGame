using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    private GameState_PlayerTurn m_playerTurn = null;

    protected override void Start()
    {
        base.Start();
        m_playerTurn = m_gameController.GetComponent<GameState_PlayerTurn>();
    }

    public override void PerformAction(Agent agent)
    {
        if (agent.m_agentInventory.AgentHasItem(m_requiredItem))
        {
            m_playerTurn.m_objectiveAchived = true;
        }
    }
}
