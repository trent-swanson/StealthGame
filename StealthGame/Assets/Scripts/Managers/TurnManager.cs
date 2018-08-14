using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	public List<Agent> m_playerTeam = new List<Agent>();

    public List<Agent> m_AITeam = new List<Agent>();

    public List<Agent> m_turnTeam = new List<Agent>();

    public enum TEAM {PLAYER, AI };
    public TEAM m_currentTeam = TEAM.PLAYER;

    static SquadManager m_squadManager;
    private UIController m_UIController = null;

    /*
    * Important Note:
    * Check any code relating to agents being dead and removing them from turn manager
    * Agents can be knocked out, but then revived so must be readded
    */

    void Start()
    {
        //Find all tiles in level and add them to GameManager tile list
        m_squadManager = GetComponent<SquadManager>();
        m_UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        InitTeamTurnMove();
    }

    private void Update()
    {
        if (m_turnTeam.Count > 0 && m_turnTeam[0] != null)
            m_turnTeam[0].AgentTurnUpdate();
        else
            InitTeamTurnMove();
    }

    //initilise unit team
    private void InitTeamTurnMove()
    {
        if (m_currentTeam == TEAM.PLAYER)
        {
            m_currentTeam = TEAM.AI;
            m_turnTeam = new List<Agent>(m_AITeam);
        }
        else
        {
            m_currentTeam = TEAM.PLAYER;
            m_turnTeam = new List<Agent>(m_playerTeam);

            m_UIController.InitUIPortraits(m_turnTeam);
        }

        m_UIController.TurnStart(m_currentTeam);

        foreach (Agent agent in m_turnTeam)
        {
            agent.AgentTurnInit();
        }

        if (ValidTeam())
        {
            m_turnTeam[0].AgentSelected();
        }
        else
        {
            InitTeamTurnMove();
        }
    }

    //end of unit turn
    public void EndUnitTurn(Agent agent)
    {
        if (ValidTeam())
        {
            SwapAgents(GetNextTeamAgentIndex());
        }
        else
            InitTeamTurnMove();
    }

    //end of team turn
    public void EndTeamTurn()
    {
        foreach (Agent agent in m_turnTeam)
        {
            PlayerController playerController = agent.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.m_currentActionPoints = 0;
                playerController.AgentTurnEnd();
            }
        }
        InitTeamTurnMove();
    }

    public void SwapAgents(int agentIndex)
    {
        if (agentIndex < m_turnTeam.Count)
        {
            m_turnTeam[0].AgentTurnEnd();

            Agent tempAgent = m_turnTeam[0];
            m_turnTeam[0] = m_turnTeam[agentIndex];
            m_turnTeam[agentIndex] = tempAgent;

            m_turnTeam[0].AgentSelected();

            if(m_currentTeam == TEAM.PLAYER)
                m_UIController.InitUIPortraits(m_turnTeam);
        }
    }

    public List<Agent> GetOpposingTeam(TEAM team)
    {
        if (team == TEAM.PLAYER)
            return m_AITeam;
        return m_playerTeam;
    }

    private bool ValidTeam()
    {
        foreach (Agent agent in m_turnTeam)
        {
            if(!agent.m_knockedout && agent.m_currentActionPoints > 0)
                return true;
        }
        return false;
    }

    private int GetNextTeamAgentIndex()
    {
        for (int i = 0; i < m_turnTeam.Count; i++)
        {
            if (!m_turnTeam[i].m_knockedout && m_turnTeam[i].m_currentActionPoints > 0)
                return i;
        }
        return 0;
    }
}
