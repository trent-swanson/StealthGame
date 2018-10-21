using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    const int INVENTORY_SLOT_COUNT = 4;

    public InventorySlot[] m_inventorySlots = new InventorySlot[INVENTORY_SLOT_COUNT];
    public TextMeshProUGUI m_itemDescription = null;

    public void Start()
    {
        for (int i = 0; i < INVENTORY_SLOT_COUNT; i++)
        {
            m_inventorySlots[i].SetupSlot(this, i);
        }
    }

    public void UpdateInventory(AgentInventory attachedInventory)
    {
        List<Item> items = attachedInventory.m_currentItems;

        for (int i = 0;i < INVENTORY_SLOT_COUNT; i++)
        {
            if (i < items.Count)
                m_inventorySlots[i].EnableSlot(items[i]);
            else
                m_inventorySlots[i].gameObject.SetActive(false);
        }
    }

    public void EnableItemDescription(int slotIndex)
    {
        m_itemDescription.text = m_inventorySlots[slotIndex].m_item.m_description;
    }

    public void DisableItemDescription()
    {
        m_itemDescription.text = "";
    }
}
