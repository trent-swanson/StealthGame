using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    public Item.ITEM_TYPE m_requiredItem = Item.ITEM_TYPE.GOLD;

    protected override void Start()
    {
        base.Start();

        if (m_interactionNode.Count == 0)
            print("Exit door has no interactable nodes");
    }

    public override void PerformAction(Agent agent)
    {
        if(agent.m_agentInventory.AgentHasItem(m_requiredItem))
            Debug.Log("OpeningDoor");
        else
            Debug.Log("Player has no gold :(");
    }

    public override NavNode GetInteractableNode(Agent agent)
    {
        return m_interactionNode[0];
    }
}
