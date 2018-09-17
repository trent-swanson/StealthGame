using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : Interactable
{
    public Item m_item = null;

    public int m_health = 3;

    protected override void Start()
    {
        base.Start();

        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up + transform.forward, Vector3.down, out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))
        {
            m_interactionNode.Add(hit.collider.GetComponent<NavNode>());
        }
    }

    public override void PerformAction(Agent agent)
    {
        if (m_health > 0)
        {
            if (agent.m_agentInventory.AgentHasItem(Item.ITEM_TYPE.CROWBAR))//Instant opening with crowbar
                m_health = 0;
            else
                m_health--;

            agent.m_currentActionPoints = 0;
        }
        else
        {
            Item newItem = Instantiate(m_item);
            newItem.EquipItem(agent);
        }
    }

    public override NavNode GetInteractableNode(Agent agent)
    {
        return m_interactionNode[0];
    }
}
