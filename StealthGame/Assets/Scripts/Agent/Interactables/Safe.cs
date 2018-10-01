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
    }

    public override void PerformAction(Agent agent)
    {
        if (agent.m_agentInventory.AgentHasItem(Item.ITEM_TYPE.CROWBAR))//Instant opening with crowbar
            m_health = 0;
        else
            m_health--;

        if (m_health <= 0)
        {
            Item newItem = Instantiate(m_item);
            newItem.EquipItem(agent);
            GetComponentInChildren<Animator>().SetTrigger("Safe_Open");
            DisableInteractable();
        }

        //After interaction, remove all points 
        agent.m_currentActionPoints = 0;
    }
}
