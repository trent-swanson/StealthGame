using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float m_minShowingTime = 2.0f;
    private Animator m_animator;

    private bool m_desiredState = false;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_desiredState = true;
        m_animator.SetBool("ShowInventory", true);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        m_desiredState = false;
        StartCoroutine(AwaitMinTime(m_minShowingTime));
    }

    private IEnumerator AwaitMinTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(!m_desiredState)
            m_animator.SetBool("ShowInventory", false);
    }
}
