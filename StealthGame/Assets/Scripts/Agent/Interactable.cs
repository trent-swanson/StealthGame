using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool m_usable = true;

    public List<NavNode> m_interactionNodes = new List<NavNode>();

    public Animation m_animation = null;

    protected virtual void Start()
    {
        foreach (NavNode interactionNode in m_interactionNodes)
        {
            if (interactionNode != null)
            {
                interactionNode.SetNodeAsInteractable(this);
            }
        }
    }

    public virtual void PerformAction(Agent agent)
    {

    }

    public void DisableInteractable()
    {
        m_usable = false;

        foreach (NavNode interactionNode in m_interactionNodes)
        {
            if (interactionNode != null)
            {
                interactionNode.m_interactable = null;
                interactionNode.SetupNodeType();
            }
        }
    }
}
