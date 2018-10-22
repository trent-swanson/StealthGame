using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour {

    public bool m_soloMode = false;

    public List<Image> m_portraitImages = new List<Image>();
    public List<TextMeshProUGUI> m_APText =  new List<TextMeshProUGUI>();

    public GameState_PlayerTurn m_playerTurn = null;

    public float m_turnStartFadeTime = 1.0f;

    public Image m_playerTurnStart = null;
    public Image m_enemyTurnStart = null;

    public Button m_endNextBtn = null;
    private Text m_endNextBtnText = null;

    private string m_endTurnString = "End Turn";
    private string m_nextPlayerString = "Next";

    private bool m_UIInteractivity = true;

    private Vector4 m_fadedColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
    private Vector4 m_fullColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

    [SerializeField]
    private List<Interactable> m_levelInteractables = new List<Interactable>();

    //Very big hack but hey it works
    public Image m_UIBlocker = null;

    void Start()
    {
        if (m_endNextBtn != null)
            m_endNextBtnText = m_endNextBtn.GetComponentInChildren<Text>();
        #if UNITY_EDITOR
        else
            Debug.Log("End turn button has not been set in the UI controller");
#endif
        m_playerTurn = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState_PlayerTurn>();

        m_levelInteractables.AddRange(Object.FindObjectsOfType<Interactable>());

        if (m_soloMode)
            m_endNextBtnText.text = m_endTurnString;
    }

    public void UpdateUI(List<Agent> agents)
    {
        int currentPlayer = m_playerTurn.m_currentAgentIndex;

        //Update live/dead
        for (int i = 0; i < agents.Count; i++)
        {
            //Update live/dead
            if (agents[i].m_knockedout)
                m_portraitImages[i].color = m_fadedColor;
            else
                m_portraitImages[i].color = m_fullColor;

            //Update AP
            m_APText[i].text = agents[i].m_currentActionPoints.ToString();
        }
    }

    public void UpdateItemInventory(Agent agent)
    {
           
    }

    public void SetUIInteractivity(bool togleVal)
    {
        if (togleVal != m_UIInteractivity)
        {
            m_UIInteractivity = togleVal;
            m_UIBlocker.enabled = !togleVal;
        }
    }

    public void ChangeAgent(int index)
    {
        if (index != m_playerTurn.m_currentAgentIndex) // Only swap player when selecting new player and is players turn
            m_playerTurn.SwapAgents(index);

        SwapEndTurnButton();
    }

    public void SwapEndTurnButton()
    {
        m_endNextBtnText.text = m_endTurnString;
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TurnStart(Agent.TEAM team)
    {
        if(team == Agent.TEAM.PLAYER && m_playerTurnStart != null)
        {
            FadeTurnStart(m_playerTurnStart);

            if(m_soloMode)
                m_endNextBtnText.text = m_endTurnString;
            else
                m_endNextBtnText.text = m_nextPlayerString;
        }
        else if(team == Agent.TEAM.NPC && m_enemyTurnStart != null)
        {
            FadeTurnStart(m_enemyTurnStart);
        }
    }

    public void EndNextBtn()
    {
        if(m_endNextBtnText.text == m_nextPlayerString)
        {
            m_playerTurn.NextPlayer();
            m_endNextBtnText.text = m_endTurnString; 
        }
        else
            m_playerTurn.EndTurn();
    }

    public void FadeTurnStart(Image imageToFade)
    {
        Color spriteColor = imageToFade.color;
        spriteColor.a = 1;
        imageToFade.color = spriteColor;

        StartCoroutine(FadeTurnLogo(Time.deltaTime / m_turnStartFadeTime, imageToFade));//Turn fade
    }

    public IEnumerator FadeTurnLogo(float time, Image imageToFade)
    {
        yield return new WaitForSeconds(time);
        Color spriteColor = imageToFade.color;
        spriteColor.a -= time;
        imageToFade.color = spriteColor;

        if(spriteColor.a > 0.05f)
        {
            StartCoroutine(FadeTurnLogo(Time.deltaTime / m_turnStartFadeTime, imageToFade));
        }
        else
        {
            spriteColor.a = 0;
            imageToFade.color = spriteColor;
        }
    }

    public void ShowInteractables(AgentInventory agentInventory)
    {
        foreach (Interactable interactable in m_levelInteractables)
        {
            if(interactable.m_usable)
            {
                if(agentInventory.AgentHasItem(interactable.m_requiredItem))//Player has the required item, highlight canvas
                {
                    interactable.FullCanvas();
                }
                else//Player does not have the required item, fade canvas
                {
                    interactable.FadeCanvas();
                }
            }
        }
    }
}
