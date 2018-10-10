using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState_GameOver : GameState
{
    public override bool UpdateState()
    {
        return true;
    }

    public override void StartState()
    {

    }

    public override void EndState()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override bool IsValid()
    {
        return true;
    }
}
