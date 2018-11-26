using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentInventory : MonoBehaviour
{
    public List<Item> m_currentItems = new List<Item>();

    private InventoryUI m_inventoryUI = null;
    private UIController m_UIController = null;

    //--------------------------------------------------------------------------------------
    // Initialisation
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        m_inventoryUI = GetComponent<Agent>().m_agentInventoryUI;
        if(m_inventoryUI!=null)
            m_inventoryUI.UpdateInventory(this);

        m_UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
    }

    //--------------------------------------------------------------------------------------
    // Determine if agent has a given item
    // 
    // Param
    //		item: item to check in inventory
    // Return:
    //      false when not in inventory
    //--------------------------------------------------------------------------------------
    public bool AgentHasItem(Item item)
    {
        foreach (Item currentItem in m_currentItems)
        {
            if (currentItem.m_itemType == item.m_itemType)
                return true;
        }
        return false;
    }

    //--------------------------------------------------------------------------------------
    // Modify agent inventory
    // TODO, split to two funcitons
    // 
    // Param
    //		functionType: should we add or remove an item
    //		item: item to add/remove
    //--------------------------------------------------------------------------------------
    public void ItemManagement(ADD_REMOVE_FUNCTION functionType, Item item)
    {
        switch (functionType)
        {
            case ADD_REMOVE_FUNCTION.ADD:
                if (!m_currentItems.Contains(item))
                {
                     m_currentItems.Add(item);
                }
                break;
            case ADD_REMOVE_FUNCTION.REMOVE:
                m_currentItems.Remove(item);
                break;
            default:
                break;
        }

        m_inventoryUI.UpdateInventory(this);
        m_UIController.ShowInteractables(this);
    }
}
