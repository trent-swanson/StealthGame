using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIController m_UIController = null;

    [HideInInspector]
    public int m_slotIndex = 0;
    private void Start()
    {
        m_UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_UIController.UpdateItemDescription(m_slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_UIController.UpdateItemDescription(-1);
    }
}
