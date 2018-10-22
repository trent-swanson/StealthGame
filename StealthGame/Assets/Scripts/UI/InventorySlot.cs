using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryUI m_parentInventory = null;
    private int m_slotIndex = 0;
    private Image m_itemImage = null;

    public Item m_item = null;

    public void SetupSlot(InventoryUI parentInventory, int slotIndex)
    {
        m_parentInventory = parentInventory;
        m_slotIndex = slotIndex;

        m_itemImage = GetComponent<Image>();
    }

    public void EnableSlot(Item item)
    {
        gameObject.SetActive(true);
        m_item = item;

        m_itemImage.sprite = m_item.m_icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Removed becuase of not knowing where to put it
        //if (m_item!=null)
        //    m_parentInventory.EnableItemDescription(m_slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {        
        //Removed becuase of not knowing where to put it
        //m_parentInventory.DisableItemDescription();
    }
}
