using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    public int m_requiredPlayerCount = 2;
    private GameState_PlayerTurn m_playerTurn = null;

    protected override void Start()
    {
        base.Start();
        m_playerTurn = m_gameController.GetComponent<GameState_PlayerTurn>();

        if (m_interactionNodes.Count < m_requiredPlayerCount)
            m_requiredPlayerCount = m_interactionNodes.Count;
    }

    public override bool CanPerform(Agent agent)
    {
        int playerCount = 0;
        bool haveItem = false;

        foreach (NavNode interactableNode in m_interactionNodes)
        {
            if (interactableNode.m_obstructingAgent != null && interactableNode.m_obstructingAgent.m_team == Agent.TEAM.PLAYER)
            {
                playerCount++;
                if (agent.m_agentInventory.AgentHasItem(m_requiredItem))
                    haveItem = true;
            }
        }

        return haveItem && playerCount >= m_requiredPlayerCount;
    }

    public override void PerformAction(Agent agent)
    {
        base.PerformAction(agent);
        m_playerTurn.m_objectiveAchived = true;
    }
}
