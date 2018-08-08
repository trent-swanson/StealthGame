using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public List<GameObject> m_inventorySlots = new List<GameObject>();

	[Space]
	public GameObject m_inventorySlotInfo;
	public List<Image> m_portraitImages;

    Agent m_currentSelectedAgent;

	TurnManager m_turnManager;

    public float m_turnStartFadeTime = 1.0f;

    public Image m_playerTurnStart = null;
    public Image m_enemyTurnStart = null;

    void Start()
    {
		m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
	}

    public void InitUIPortraits(List<Agent> agents)
    {
        foreach (Image portraitImage in m_portraitImages)
        {
            portraitImage.enabled = false ;
        }
        for (int i = 0; i < agents.Count && i < m_portraitImages.Count; i++)
        {
            m_portraitImages[i].enabled = true;
            m_portraitImages[i].sprite = agents[i].GetComponent<PlayerController>().portrait;
        }
    }

	public void UpdateUI(Agent p_agent) {
		m_currentSelectedAgent = p_agent;
		ResetUI();
		GetInventoryItems();
	}

	public void ResetUI() {
		foreach (GameObject slot in m_inventorySlots) {
			slot.SetActive(false);
		}
	}

	void GetInventoryItems() {
		for (int i = 0; i < m_currentSelectedAgent.m_currentItems.Count; i++) {
			m_inventorySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = m_currentSelectedAgent.m_currentItems[i].icon;
			m_inventorySlots[i].SetActive(true);
		}
	}

	public void AddItem(Agent p_agent) {
		if (p_agent == m_currentSelectedAgent) {
			int index = p_agent.m_currentItems.Count - 1;
			m_inventorySlots[index].transform.GetChild(0).GetComponent<Image>().sprite = p_agent.m_currentItems[index].icon;
			m_inventorySlots[index].SetActive(true);
		}
	}

	public void inventorySlotHover(int slotID) {
		m_inventorySlotInfo.transform.GetChild(0).GetComponent<Text>().text = m_currentSelectedAgent.m_currentItems[slotID].description;
		m_inventorySlotInfo.SetActive(true);
	}

	public void inventorySlotHoverExit() {
		m_inventorySlotInfo.SetActive(false);
	}

    public void ChangeAgent(int index)
    {
        if (index != 0 && m_turnManager.m_currentTeam == TurnManager.TEAM.PLAYER) // Only swap player when selecting new player and is players turn
            m_turnManager.SwapAgents(index);
    }

    public void TurnStart(TurnManager.TEAM team)
    {
        if(team == TurnManager.TEAM.PLAYER)
        {
            Color spriteColor = m_playerTurnStart.color;
            spriteColor.a = 1;
            m_playerTurnStart.color = spriteColor;

            StartCoroutine(FadeTurnStart(0.2f, m_playerTurnStart));
        }
        else
        {
            Color spriteColor = m_enemyTurnStart.color;
            spriteColor.a = 1;
            m_enemyTurnStart.color = spriteColor;

            StartCoroutine(FadeTurnStart(Time.deltaTime / m_turnStartFadeTime, m_enemyTurnStart));
        }
    }

    public IEnumerator FadeTurnStart(float time, Image imageToFade)
    {
        yield return new WaitForSeconds(time);
        Color spriteColor = imageToFade.color;
        spriteColor.a -= time;
        imageToFade.color = spriteColor;

        if(spriteColor.a > 0.05f)
        {
            StartCoroutine(FadeTurnStart(Time.deltaTime / m_turnStartFadeTime, imageToFade));
        }
        else
        {
            spriteColor.a = 0;
            imageToFade.color = spriteColor;
        }
    }
}
