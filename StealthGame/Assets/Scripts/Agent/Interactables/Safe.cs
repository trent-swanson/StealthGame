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
        if (agent.m_agentInventory.AgentHasItem(m_requiredItem))
        {
            Item newItem = Instantiate(m_item);
            newItem.EquipItem(agent);
            GetComponentInChildren<Animator>().SetTrigger("Safe_Open");
            DisableInteractable();

            //After interaction, remove all points 
            agent.m_currentActionPoints = 0;
        }
    }
}
