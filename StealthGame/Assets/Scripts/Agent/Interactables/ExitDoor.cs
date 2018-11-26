using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    public int m_requiredPlayerCount = 2;
    private GameState_PlayerTurn m_playerTurn = null;

    //--------------------------------------------------------------------------------------
    // Initialisation
    // Ensure interactable has a conected canvas
    // Set navigation nodes to be a item based interatable 
    //
    // Set up visual ques 
    //--------------------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();
        m_playerTurn = m_gameController.GetComponent<GameState_PlayerTurn>();

        if (m_interactionNodes.Count < m_requiredPlayerCount)
            m_requiredPlayerCount = m_interactionNodes.Count;
    }

    //--------------------------------------------------------------------------------------
    // Can agent use this interactable
    // Requires both agents to be on a nav node, with one having a requred item
    //
    // Param
    //		agent: agent to check condition against
    // Return:
    //      bool when usable
    //--------------------------------------------------------------------------------------
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

    //--------------------------------------------------------------------------------------
    // Perform interactables actions
    // Set objective achieved as true
    //
    // Param
    //		agent: agent to perform action
    //--------------------------------------------------------------------------------------
    public override void PerformAction(Agent agent)
    {
        base.PerformAction(agent);
        m_playerTurn.m_objectiveAchived = true;

        PlayDoorOpening();
    }

    //--------------------------------------------------------------------------------------
    // Play sounds
    //--------------------------------------------------------------------------------------
    public void PlayDoorOpening()
    {
        m_soundController.PlaySound(SoundController.SOUND.DOOR_ENTRANCE_0);
    }
}
