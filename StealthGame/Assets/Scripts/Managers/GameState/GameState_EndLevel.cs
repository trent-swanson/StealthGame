using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState_EndLevel : GameState
{
    public string m_nextLevel = "";

    private GameState_TurnManager m_turnManager = null;

    private void Start()
    {
        m_turnManager = GetComponent<GameState_TurnManager>();
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
        SceneManager.LoadScene(m_nextLevel);
    }

    public override bool IsValid()
    {
        return !m_turnManager.GameOver();
    }
}
