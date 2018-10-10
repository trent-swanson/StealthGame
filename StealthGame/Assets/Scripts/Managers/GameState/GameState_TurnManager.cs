using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_TurnManager : GameState
{
	public List<Agent> m_playerTeam = new List<Agent>();

    public List<Agent> m_NPCTeam = new List<Agent>();

    public List<Agent> m_turnTeam = new List<Agent>();
    public Agent.AGENT_UPDATE_STATE m_currentAgentState = Agent.AGENT_UPDATE_STATE.END_TURN;

    public enum TEAM {PLAYER, AI };
    public TEAM m_currentTeam = TEAM.PLAYER;

    static SquadManager m_squadManager;
    private UIController m_UIController = null;

    public int m_autoStandupTime = 2;

    public float m_pressure = 0.0f;

    public bool m_objectiveAchived = false;

    /*
    * Important Note:
    * Check any code relating to agents being dead and removing them from turn manager
    * Agents can be knocked out, but then revived so must be readded
    */

    private void Start()
    {
        //Find all tiles in level and add them to GameManager tile list
        m_squadManager = GetComponent<SquadManager>();
        m_UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
    }

    //------Game state------

    
    public override bool UpdateState()
    {
        m_currentAgentState = m_turnTeam[0].AgentTurnUpdate();

        //Super basic state machine for turn management
        switch (m_currentAgentState)
        {
            case Agent.AGENT_UPDATE_STATE.AWAITING_INPUT:
                m_UIController.SetUIInteractivity(true);
                break;
            case Agent.AGENT_UPDATE_STATE.PERFORMING_ACTIONS:
                m_UIController.SetUIInteractivity(false);
                break;
            case Agent.AGENT_UPDATE_STATE.END_TURN:
                EndUnitTurn(m_turnTeam[0]);

                if(!ValidTeam())
                    InitTeamTurnMove();

                m_UIController.SetUIInteractivity(true);
                break;
            default:
                break;
        }

        return (m_objectiveAchived || GameOver());
    }

    public override void StartState()
    {
        foreach (Agent NPCAgent in m_NPCTeam)
        {
            NPC NPCScript = NPCAgent.GetComponent<NPC>();
            if(NPCScript != null)
            {
                NPCScript.BuildVision();
            }
        }

        InitTeamTurnMove();
    }

    public override void EndState()
    {

    }

    public override bool IsValid()
    {
        return true;
    }

    //------End Game state------

    //initilise unit team
    private void InitTeamTurnMove()
    {
        if (m_currentTeam == TEAM.PLAYER)
        {
            m_currentTeam = TEAM.AI;
            m_turnTeam = new List<Agent>(m_NPCTeam);
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

        if (!ValidTeam())
        {
            InitTeamTurnMove();
        }

        m_turnTeam[0].AgentSelected();
    }

    //end of unit turn
    public void EndUnitTurn(Agent agent)
    {
        agent.AgentTurnEnd();

        if (ValidTeam())
        {
            NextPlayer();
        }
        else
            InitTeamTurnMove();
    }

    //Ends the current teams turn
    //Called by UI button
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

    public void NextPlayer()
    {
        SwapAgents(GetNextTeamAgentIndex());
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
            return m_NPCTeam;
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
        for (int i = 1; i < m_turnTeam.Count; i++)
        {
            if (!m_turnTeam[i].m_knockedout && m_turnTeam[i].m_currentActionPoints > 0)
                return i;
        }
        return 0;
    }

    public void UpdateNPCWorldStates()
    {
        for (int i = 0; i < m_NPCTeam.Count; i++)
        {
            if (!m_NPCTeam[i].m_knockedout)
                m_NPCTeam[i].GetComponent<NPC>().UpdateWorldState();
        }
    }

    public bool GameOver()
    {
        foreach (Agent agent in m_playerTeam)
        {
            if (!agent.m_knockedout)
                return false;
        }
        return true;
    }
}
