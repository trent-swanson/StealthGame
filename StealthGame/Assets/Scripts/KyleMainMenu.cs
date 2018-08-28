using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class KyleMainMenu : MonoBehaviour {

    public GameObject inGameUI;
    public GameObject mainMenuUI;

    [Header("Cameras")]
    public GameObject mainCamera;
    public GameObject mainMenuCamera;

    // Use this for initialization
    void Start () {
        //Freezes Game
        Time.timeScale = 0;

        //Turns off In-Game UI
        inGameUI.SetActive(false);

        //Turns off MM camera
        mainCamera.SetActive(false);
	}

    private bool transitioning = false;

    public void StartGame()
    {
        //Resumes Time
        Time.timeScale = 1;

        //Turns of Main Menu UI
        mainMenuUI.SetActive(false);

        TriggerAnimation();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void TriggerAnimation()
    {
        Animator anim = mainMenuCamera.GetComponent<Animator>();
        anim.SetTrigger("CameraTrans");

        StartCoroutine(ExecuteAfterTime(3.1f));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        //suspend execution for X seconds
        yield return new WaitForSeconds(time);

        //Swaps Cinematic camera with Gameplay camera
        mainCamera.SetActive(true);
        mainMenuCamera.SetActive(false);
        //Turns on In-Game UI
        inGameUI.SetActive(true);
    }

    
}
