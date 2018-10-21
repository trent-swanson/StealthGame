using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState_GameOver : GameState
{
    private GameState_PlayerTurn m_playerTurn = null;

    private void Start()
    {
        m_playerTurn = GetComponent<GameState_PlayerTurn>();
    }

    public override bool UpdateState()
    {
        return true;
    }

    public override void StartState()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override void EndState()
    {
    }

    public override bool IsValid()
    {
        return !m_playerTurn.TeamStillAlive();
    }
}
