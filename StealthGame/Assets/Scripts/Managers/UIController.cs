using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public List<GameObject> inventorySlots = new List<GameObject>();

	[Space]
	public GameObject inventorySlotInfo;
	public Image portraitImage;
	Agent currentSelectedAgent;

	public void UpdateUI(Agent p_agent) {
		currentSelectedAgent = p_agent;
		ResetUI();
		GetInventoryItems();
		portraitImage.sprite = p_agent.GetComponent<PlayerController>().portrait;
	}

	public void ResetUI() {
		foreach (GameObject slot in inventorySlots) {
			slot.SetActive(false);
		}
	}

	void GetInventoryItems() {
		for (int i = 0; i < currentSelectedAgent.currentItems.Count; i++) {
			inventorySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = currentSelectedAgent.currentItems[i].icon;
			inventorySlots[i].SetActive(true);
		}
	}

	public void AddItem(Agent p_agent) {
		if (p_agent == currentSelectedAgent) {
			int index = p_agent.currentItems.Count - 1;
			inventorySlots[index].transform.GetChild(0).GetComponent<Image>().sprite = p_agent.currentItems[index].icon;
			inventorySlots[index].SetActive(true);
		}
	}
	
	public void EndTurn() {
		TurnManager.EndPlayerTurn();
	}

	public void inventorySlotHover(int slotID) {
		inventorySlotInfo.transform.GetChild(0).GetComponent<Text>().text = currentSelectedAgent.currentItems[slotID].description;
		inventorySlotInfo.SetActive(true);
	}

	public void inventorySlotHoverExit() {
		inventorySlotInfo.SetActive(false);
	}
}
