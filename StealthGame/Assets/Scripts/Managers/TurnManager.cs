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

    /*
    * Important Note:
    * Check any code relating to agents being dead and removing them from turn manager
    * Agents can be knocked out, but then revived so must be readded
    */

    void Start()
    {
        //Find all tiles in level and add them to GameManager tile list
        GameManager.tiles = GameObject.FindGameObjectsWithTag("Tile");
        m_squadManager = GetComponent<SquadManager>();
    }

    private void Update()
    {
        if (m_turnTeam.Count != 0)
        {
            if (m_currentTeam == TEAM.PLAYER)
            {
                m_turnTeam[0].TurnUpdate();
            }
            else // Enemy turn
            {
                List<Agent> tempTeam = new List<Agent>(m_turnTeam);
                foreach (Agent agent in tempTeam)
                {
                    agent.TurnUpdate();
                }
            }
        }
        else
        {
            InitTeamTurnMove();
        }
    }

    //initilise unit team
    private void InitTeamTurnMove()
    {
        Debug.Log("TeamSwap");
        if (m_currentTeam == TEAM.PLAYER)
        {
            m_currentTeam = TEAM.AI;
            m_turnTeam = new List<Agent>(m_AITeam);
        }
        else
        {
            m_currentTeam = TEAM.PLAYER;
            m_turnTeam = new List<Agent>(m_playerTeam);
        }
        if (m_turnTeam.Count > 0)
        {
            
            for (int i = 0; i < m_turnTeam.Count;)
            {
                m_turnTeam[i].m_currentActionPoints = m_turnTeam[i].m_maxActionPoints;

                if (m_turnTeam[i].m_knockedout)
                    m_turnTeam.RemoveAt(i);
                else
                {
                    m_turnTeam[i].StartUnitTurn();
                    i++;
                }
            }
        }
    }

    //end of unit turn
    public void EndUnitTurn(Agent agent)
    {
        m_turnTeam.Remove(agent);
    }

    public List<Agent> GetOpposingTeam(TEAM team)
    {
        if (team == TEAM.PLAYER)
            return m_AITeam;
        return m_playerTeam;
    }
}
