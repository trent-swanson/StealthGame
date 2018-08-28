using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class KyleMainMenu : MonoBehaviour {

    public GameObject inGameUI;
    public GameObject mainMenuUI;

	// Use this for initialization
	void Start () {
        //Freezes Game
        Time.timeScale = 0;

        //Turns off In-Game UI
        inGameUI.SetActive(false);
	}

    public void StartGame()
    {
        //Resumes Time
        Time.timeScale = 1;

        //Turns on In-Game UI
        inGameUI.SetActive(true);

        //Turns of Main Menu UI
        mainMenuUI.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
