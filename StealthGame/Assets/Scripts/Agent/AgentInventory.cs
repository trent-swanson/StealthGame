using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentInventory : MonoBehaviour
{
    public List<Item> m_currentItems = new List<Item>();

    private InventoryUI m_inventoryUI = null;

    private void Start()
    {
        m_inventoryUI = GetComponent<Agent>().m_agentInventoryUI;
        if(m_inventoryUI!=null)
            m_inventoryUI.UpdateInventory(this);
    }

    public bool AgentHasItem(Item item)
    {
        foreach (Item currentItem in m_currentItems)
        {
            if (currentItem.m_itemType == item.m_itemType)
                return true;
        }
        return false;
    }

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
    }
}
