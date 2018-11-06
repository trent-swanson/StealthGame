using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameController : MonoBehaviour
{
    public GameState m_currentState = null;
    public Agent.TEAM m_currentTeam = Agent.TEAM.NPC;
    private void Update()
    {
        //State completed
        if(m_currentState.UpdateState())
        {
            //Get next valid state
            foreach (GameState gameState in m_currentState.m_nextStates)
            {
                if(gameState.IsValid())
                {
                    SwapState(gameState);
                    break;
                }
            }
        }
    }

    private void SwapState(GameState nextState)
    {
        m_currentState.EndState();
        m_currentState = nextState;
        m_currentState.StartState();
    }

}
