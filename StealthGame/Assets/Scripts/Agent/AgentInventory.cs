using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentInventory : MonoBehaviour
{
    public List<Item> m_currentItems = new List<Item>();
    private Agent m_agent = null;

    private void Start()
    {
        m_agent = GetComponent<Agent>();
    }
    public bool AgentHasItem(Item.ITEM_TYPE itemType)
    {
        foreach (Item item in m_currentItems)
        {
            if (item.m_itemType == itemType)
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
                    GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>().inventoryAnimation.PopupInventory();
                }
                break;
            case ADD_REMOVE_FUNCTION.REMOVE:
                m_currentItems.Remove(item);
                break;
            default:
                break;
        }
    }
}
