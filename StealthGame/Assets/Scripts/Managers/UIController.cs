using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public List<GameObject> m_inventorySlots = new List<GameObject>();

	[Space]
	public GameObject m_inventorySlotInfo;
	public Image m_portraitImage;
	Agent m_currentSelectedAgent;

	TurnManager turnManager;

	void Start() {
		turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
	}

	public void UpdateUI(Agent p_agent) {
		m_currentSelectedAgent = p_agent;
		ResetUI();
		GetInventoryItems();
		m_portraitImage.sprite = p_agent.GetComponent<PlayerController>().portrait;
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
	
	//public void EndTurn() {
	//	turnManager.EndUnitTurn();
	//}

	public void inventorySlotHover(int slotID) {
		m_inventorySlotInfo.transform.GetChild(0).GetComponent<Text>().text = m_currentSelectedAgent.m_currentItems[slotID].description;
		m_inventorySlotInfo.SetActive(true);
	}

	public void inventorySlotHoverExit() {
		m_inventorySlotInfo.SetActive(false);
	}
}
