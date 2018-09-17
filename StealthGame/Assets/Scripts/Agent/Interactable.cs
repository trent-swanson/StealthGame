using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public List<NavNode> m_interactionNode = null;
    public NavNode m_currentNode = null;

    public Animation m_animation = null;

    protected virtual void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, Vector3.down, out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))
        {
            m_currentNode = hit.collider.GetComponent<NavNode>();
            m_currentNode.NavNodeInteractable(ADD_REMOVE_FUNCTION.ADD, this);
        }
    }

    public virtual void PerformAction(Agent agent)
    {

    }

    public virtual NavNode GetInteractableNode(Agent agent)
    {
        return m_currentNode;
    }
}
