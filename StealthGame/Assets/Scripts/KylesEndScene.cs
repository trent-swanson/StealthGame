using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KylesEndScene : MonoBehaviour {

    public GameObject steamGauge;

    [Header("Triggers")]
    public GameObject entrance;
    public GameObject exit;
    public GameObject safe;
    [HideInInspector]
    public bool safeLooted = false;
    public bool triggerEntered = false;

    private List<GameObject> players = new List<GameObject>();

	// Use this for initialization
	void Start () {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
        }
    }
	
	// Update is called once per frame
	void Update () {

        CheckForGameEnd();

	}

    void CheckForGameEnd()
    {
        //Player has entered a Door
        if (triggerEntered)
        {
            //Safe has been looted
            if (safeLooted)
            {
                RestartLevel();
            }
            triggerEntered = false;
            SteamChange();
        }
    }

    void RestartLevel()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);

        Debug.Log("Restarted Level");
    }

    void SteamChange()
    {
        if (!triggerEntered || !safeLooted)
        {
            steamGauge.GetComponent<KyleSteamGauge>().steamValue += 5f;
        }
    }

}
