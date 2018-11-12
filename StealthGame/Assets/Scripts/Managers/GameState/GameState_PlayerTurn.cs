using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_PlayerTurn : GameState_Turn
{
    [HideInInspector]
    public bool m_objectiveAchived = false;
    public float m_showDeathDelay = 2.0f; 
    private bool m_endTurn = false;

    private GameState_NPCTurn m_NPCTurn = null;

    protected override void Start()
    {
        base.Start();

        m_UIController.SetUIInteractivity(true);
        m_NPCTurn = GetComponent<GameState_NPCTurn>();
    }

    /// <returns>True when state is completed</returns>
    public override bool UpdateState()
    {
        if (Input.GetAxisRaw("Cancel") != 0)//Quit game
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.Tab))//Swap players
        {
            NextPlayer();
            return false;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))//End turn
        {
            m_UIController.EndNextBtn();
            return false;
        }

        m_currentAgentState = m_team[m_currentAgentIndex].AgentTurnUpdate();
        m_UIController.UpdateUI(m_team);

        //Super basic state machine for turn management
        switch (m_currentAgentState)
        {
            case Agent.AGENT_UPDATE_STATE.AWAITING_INPUT:
                m_UIController.SetUIInteractivity(true);
                break;
            case Agent.AGENT_UPDATE_STATE.PERFORMING_ACTIONS:
                m_UIController.SetUIInteractivity(false);
                return (false);//Early break out, ensures game continues till end of player animations
            case Agent.AGENT_UPDATE_STATE.END_TURN:
                NextPlayer();
                m_UIController.SetUIInteractivity(false);
                break;
            default:
                break;
        }

        return (m_objectiveAchived || m_endTurn);
    }

    public override void StartState()
    {
        m_currentAgentIndex = 0;
        m_gameController.m_currentTeam = Agent.TEAM.PLAYER;
        m_NPCTurn.UpdateNPCVision();
        m_NPCTurn.UpdateNPCWorldStates();

        m_endTurn = false;

        m_UIController.UpdateUI(m_team);

        if(TeamStillAlive())
        {
            foreach (Agent agent in m_team)
            {
                if (!agent.m_knockedout)
                    agent.AgentTurnInit();
                else
                    agent.m_currentActionPoints = 0;
            }

            if (m_team[m_currentAgentIndex].m_knockedout)
                SwapAgents(GetNextTeamAgentIndex());

            m_team[m_currentAgentIndex].AgentSelected();
            m_UIController.TurnStart(Agent.TEAM.PLAYER);
            m_UIController.ShowInteractables(m_team[m_currentAgentIndex].m_agentInventory);
        }
    }

    public override void EndState()
    {
        foreach (Agent agent in m_team)
        {
            PlayerController playerController = agent.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.m_currentActionPoints = 0;
                playerController.AgentTurnEnd();
            }
        }
    }

    public override bool IsValid()
    {
        return TeamStillAlive();
    }

    public void AutoEndTurn()
    {
        int nextPlayer = GetNextTeamAgentIndex();
        if(nextPlayer!= m_currentAgentIndex)//Next player is valid, swap to next player 
        {
            SwapAgents(nextPlayer);
        }
        else//Next player is not a valid choice, end turn
        {
            EndTurn();
        }
    }

    public override void SwapAgents(int agentIndex)
    {
        if (agentIndex < m_team.Count && agentIndex >= 0 && agentIndex != m_currentAgentIndex)
        {
            m_team[m_currentAgentIndex].AgentTurnEnd();

            m_currentAgentIndex = agentIndex;

            m_team[m_currentAgentIndex].AgentSelected();

            m_UIController.UpdateUI(m_team);
            m_UIController.SwapEndTurnButton();
            m_UIController.ShowInteractables(m_team[m_currentAgentIndex].m_agentInventory);
        }
    }

    //Ends the current teams turn
    //Called by UI button
    public void EndTurn()
    {
        m_endTurn = true;

        foreach (Agent agent in m_team)
        {
            PlayerController playerController = agent.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.m_currentActionPoints = 0;
            }
        }
    }
}
