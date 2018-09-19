using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool m_usable = true;

    public NavNode m_interactionNode = null;

    public Animation m_animation = null;

    protected virtual void Start()
    {
        if (m_interactionNode != null)
        {
            m_interactionNode.m_interactable = this;
            m_interactionNode.SetupNodeType();
        }
        #if UNITY_EDITOR
        else
        {
            print("Interactable node is not set");
        }
        #endif
    }

    public virtual void PerformAction(Agent agent)
    {

    }

    public virtual NavNode GetInteractableNode(Agent agent)
    {
        return m_interactionNode;
    }

    public void DisableInteractable()
    {
        m_usable = false;
        m_interactionNode.SetupNodeType();
    }
}
