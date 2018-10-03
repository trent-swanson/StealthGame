using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public List<Image> m_portraitImages;
    public InventoryAnimation inventoryAnimation = null;

    TurnManager m_turnManager;

    public float m_turnStartFadeTime = 1.0f;

    public Image m_playerTurnStart = null;
    public Image m_enemyTurnStart = null;

    public Button m_endTurnBtn = null;
    public Button m_nextPlayerBtn = null;

    private bool m_UIInteractivity = true;

    public List<GameObject> m_inventorySlots = new List<GameObject>();

    [Space]
    public GameObject m_inventorySlotInfo;

    private List<Item> m_items = new List<Item>();

    void Start()
    {
		m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();

#if UNITY_EDITOR
        if (m_endTurnBtn == null)
            Debug.Log("End turn button has not been set in the UI controller");
        if (m_nextPlayerBtn == null)
            Debug.Log("Next player button has not been set in the UI controller");
#endif
        //Initialise slot indexes
        for (int i = 0; i < m_inventorySlots.Count; i++)
        {
            m_inventorySlots[i].GetComponent<InventorySlot>().m_slotIndex = i;
        }

    }

    public void InitUIPortraits(List<Agent> agents)
    {
        foreach (Image portraitImage in m_portraitImages)
        {
            portraitImage.enabled = false;
        }
        for (int i = 0; i < agents.Count && i < m_portraitImages.Count; i++)
        {
            m_portraitImages[i].enabled = true;
            m_portraitImages[i].sprite = agents[i].GetComponent<PlayerController>().portrait;
        }
    }

    public void UpdateItemInventory(Agent agent)
    {
        m_items = agent.m_agentInventory.m_currentItems;
        int agentItemCount = m_items.Count;

        for (int i = 0; i < m_inventorySlots.Count; i++)
        {
            //Draw item
            if(i < agentItemCount)
            {
                m_inventorySlots[i].SetActive(true);
                m_inventorySlots[i].GetComponent<Image>().sprite = m_items[i].m_icon;
            }
            else
            {
                m_inventorySlots[i].SetActive(false);
            }
        }
    }

    public void UpdateItemDescription(int index)
    {
        if (index >= 0 && index < m_items.Count)
        {
            Item item = m_items[index];
            if (item != null)
                m_inventorySlotInfo.GetComponent<Text>().text = item.m_description;
        }
        else
        {
            m_inventorySlotInfo.GetComponent<Text>().text = "";
         
        }
    }

    public void SetUIInteractivity(bool togleVal)
    {
        if (togleVal != m_UIInteractivity)
        {
            m_UIInteractivity = togleVal;

            m_endTurnBtn.interactable = togleVal;
            m_nextPlayerBtn.interactable = togleVal;
        }
    }

    public void NextPlayer()
    {
        m_turnManager.NextPlayer();
    }


    public void ChangeAgent(int index)
    {
        if (m_UIInteractivity && index != 0 && m_turnManager.m_currentTeam == TurnManager.TEAM.PLAYER) // Only swap player when selecting new player and is players turn
            m_turnManager.SwapAgents(index);
    }

    public void TurnStart(TurnManager.TEAM team)
    {
        if(team == TurnManager.TEAM.PLAYER && m_playerTurnStart != null)
        {
            Color spriteColor = m_playerTurnStart.color;
            spriteColor.a = 1;
            m_playerTurnStart.color = spriteColor;

            StartCoroutine(FadeTurnStart(Time.deltaTime / m_turnStartFadeTime, m_playerTurnStart));
        }
        else if(team == TurnManager.TEAM.AI && m_enemyTurnStart != null)
        {
            Color spriteColor = m_enemyTurnStart.color;
            spriteColor.a = 1;
            m_enemyTurnStart.color = spriteColor;

            StartCoroutine(FadeTurnStart(Time.deltaTime / m_turnStartFadeTime, m_enemyTurnStart));
        }
    }

    public void TurnEnd()
    {
        m_turnManager.EndTeamTurn();
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
