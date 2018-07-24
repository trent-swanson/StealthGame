using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	public List<Agent> m_playerTeam = new List<Agent>();
    public List<Agent> m_enemyTeam = new List<Agent>();

    private List<Agent> m_turnTeam = new List<Agent>();

    private enum TEAM {PLAYER,ENEMY };
    private TEAM m_currentTeam = TEAM.PLAYER;

    enum Team { AI, PLAYER }
    Team team;

    /*
    * Important Note:
    * Check any code relating to agents being dead and removing them from turn manager
    * Agents can be knocked out, but then revived so must be readded
    */

	void Start() {
		//Find all tiles in level and add them to GameManager tile list
		GameManager.tiles = GameObject.FindGameObjectsWithTag("Tile");
	}

    private void Update()
    {
        if (m_turnTeam.Count != 0)
        {
            m_turnTeam[0].TurnUpdate();
        }
        else
        {
            InitTeamTurnMove();
        }
    }

    //initilise unit team
    private void InitTeamTurnMove()
    {
        if(m_currentTeam == TEAM.PLAYER)
        {
            m_currentTeam = TEAM.ENEMY;
            m_turnTeam = new List<Agent>(m_enemyTeam);
        }
        else
        {
            m_currentTeam = TEAM.PLAYER;
            m_turnTeam = new List<Agent>(m_playerTeam);
        }
        if (m_turnTeam.Count > 0) {
            m_turnTeam[0].StartUnitTurn();
        }
    }

    //end of unit turn
    public void EndUnitTurn()
    {
        m_turnTeam[0].EndTurn();
        m_turnTeam.RemoveAt(0);
        if(m_turnTeam.Count > 0)
            m_turnTeam[0].StartUnitTurn();
    }
}
