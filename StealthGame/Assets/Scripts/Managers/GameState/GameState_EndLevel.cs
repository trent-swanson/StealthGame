using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState_EndLevel : GameState
{
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
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public override void EndState()
    {

    }

    public override bool IsValid()
    {
        return !m_turnManager.GameOver();
    }
}
