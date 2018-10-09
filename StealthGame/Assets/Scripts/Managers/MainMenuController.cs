using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string m_firstLevel = "";

    public void StartGame()
    {
        //TODO apply camera transisition

        SceneManager.LoadScene(m_firstLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
