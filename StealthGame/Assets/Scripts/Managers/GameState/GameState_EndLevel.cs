using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState_EndLevel : GameState
{
    private GameState_PlayerTurn m_playerState = null;

    private void Start()
    {
        m_playerState = GetComponent<GameState_PlayerTurn>();
    }

    public override bool UpdateState()
    {
        return true;
    }


    public GameObject levelCompleteUI;
    public GameObject endTurnButton;
    public override void StartState()
    {
        levelCompleteUI.SetActive(true);
        endTurnButton.SetActive(false);
    }

    public override void EndState()
    {

    }

    public override bool IsValid()
    {
        return m_playerState.m_objectiveAchived;
    }

    public void SceneChange()
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
}
