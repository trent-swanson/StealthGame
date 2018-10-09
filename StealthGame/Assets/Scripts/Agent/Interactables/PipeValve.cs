using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeValve : Interactable
{
    public EnviromentHazard m_activatedHazard = null;
    public Item.ITEM_TYPE m_requiredItem = Item.ITEM_TYPE.WRENCH;
    protected override void Start()
    {
        base.Start();
    }

    public override void PerformAction(Agent agent)
    {
        if (agent.m_agentInventory.AgentHasItem(m_requiredItem))
            m_activatedHazard.ActivateHazard();
    }
}
