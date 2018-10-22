using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_NPCTurn : GameState_Turn
{
    public int m_autoStandupTime = 2;

    protected override void Start()
    {
        base.Start();
    }

    /// <returns>True when state is completed</returns>
    public override bool UpdateState()
    {
        UpdateNPCVision();

        if (Input.GetAxisRaw("Cancel") != 0)
            Application.Quit();

        m_currentAgentState = m_team[m_currentAgentIndex].AgentTurnUpdate();

        //Super basic state machine for turn management
        switch (m_currentAgentState)
        {
            case Agent.AGENT_UPDATE_STATE.AWAITING_INPUT:
                break;
            case Agent.AGENT_UPDATE_STATE.PERFORMING_ACTIONS:
                break;
            case Agent.AGENT_UPDATE_STATE.END_TURN:
                NextPlayer();
                break;
            default:
                break;
        }

        return (!TeamCanMove());
    }

    public override void StartState()
    {
        m_currentAgentIndex = 0;

        foreach (Agent agent in m_team)
        {
            if (agent.m_knockedout)
            {
                NPC NPCAgent = agent.GetComponent<NPC>();

                NPCAgent.m_autoStandupTimer--;

                if (NPCAgent.m_autoStandupTimer <= 0 && NPCAgent.m_currentNavNode.m_obstructingAgent == null)
                {
                    NPCAgent.Revive();

                    NPCAgent.m_currentActionPoints = 0;

                    //Reset all world states
                    NPCAgent.m_agentWorldState.SetInvestigationNodes(new List<NPC.InvestigationNode>());
                    NPCAgent.m_agentWorldState.SetPossibleTargets(new List<Agent>());
                }
            }
            else
            {
                agent.AgentTurnInit();
            }
        }

        if(TeamStillAlive())
        {
            if (m_team[m_currentAgentIndex].m_knockedout || m_team[m_currentAgentIndex].m_currentActionPoints == 0)
                SwapAgents(GetNextTeamAgentIndex());

            m_team[m_currentAgentIndex].AgentSelected();
            m_UIController.TurnStart(Agent.TEAM.NPC);
        }

        m_UIController.SetUIInteractivity(false);
    }

    public override void EndState()
    {

    }

    public override bool IsValid()
    {
        return true;
    }

    public override void SwapAgents(int agentIndex)
    {
        if (agentIndex < m_team.Count && agentIndex >= 0 && agentIndex != m_currentAgentIndex && !m_team[agentIndex].m_knockedout)
        {
            m_team[m_currentAgentIndex].AgentTurnEnd();

            m_currentAgentIndex = agentIndex;

            m_team[m_currentAgentIndex].AgentSelected();
        }
    }

    public void UpdateNPCVision()
    {
        foreach (Agent NPCAgent in m_team)
        {
            NPC NPCScript = NPCAgent.GetComponent<NPC>();
            if (NPCScript != null)
            {
                NPCScript.BuildVision();
            }
        }
    }

    public void UpdateNPCWorldStates()
    {
        for (int i = 0; i < m_team.Count; i++)
        {
            if (!m_team[i].m_knockedout)
                m_team[i].GetComponent<NPC>().UpdateWorldState();
        }
    }
}
