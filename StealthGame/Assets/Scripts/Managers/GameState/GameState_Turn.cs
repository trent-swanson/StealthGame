using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Turn : GameState
{
    protected UIController m_UIController = null;

    public List<Agent> m_team = new List<Agent>();
    public int m_currentAgentIndex = 0;

    public Agent.AGENT_UPDATE_STATE m_currentAgentState = Agent.AGENT_UPDATE_STATE.END_TURN;

    protected virtual void Start()
    {
        //Find all tiles in level and add them to GameManager tile list
        m_UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
    }

    public override bool UpdateState()
    {
        return true;
    }

    public override void StartState()
    {

    }

    public override void EndState()
    {

    }

    public override bool IsValid()
    {
        return true;
    }

    public virtual void SwapAgents(int agentIndex)
    {

    }

    public void NextPlayer()
    {
        SwapAgents(GetNextTeamAgentIndex());
    }

    protected int GetNextTeamAgentIndex()
    {
        for (int i = 0; i < m_team.Count; i++)
        {
            if (i != m_currentAgentIndex && !m_team[i].m_knockedout && m_team[i].m_currentActionPoints > 0)
                return i;
        }
        return m_currentAgentIndex;
    }

    public bool TeamCanMove()
    {
        foreach (Agent agent in m_team)
        {
            if (agent.m_currentActionPoints > 0)
                return true;
        }
        return false;
    }

    public bool TeamStillAlive()
    {
        foreach (Agent agent in m_team)
        {
            if (!agent.m_knockedout)
                return true;
        }
        return false;
    }
}
