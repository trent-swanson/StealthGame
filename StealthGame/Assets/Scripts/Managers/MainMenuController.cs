using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string m_firstLevel = "";

    public void StartGame()
    {
        SceneManager.LoadScene(m_firstLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadLevel(string levelString)
    {
        SceneManager.LoadScene(levelString);
    }
}
