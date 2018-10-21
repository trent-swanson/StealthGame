using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool m_usable = true;

    public Item m_requiredItem = null;

    public List<NavNode> m_interactionNodes = new List<NavNode>();

    public Animation m_animation = null;

    protected GameController m_gameController = null;

    protected virtual void Start()
    {
        foreach (NavNode interactionNode in m_interactionNodes)
        {
            if (interactionNode != null)
            {
                interactionNode.SetNodeAsInteractable(this);
            }
        }

        m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
