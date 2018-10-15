using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeValve : Interactable
{
    public List<EnviromentHazard> m_activatedHazard = new List<EnviromentHazard>();
    public Item.ITEM_TYPE m_requiredItem = Item.ITEM_TYPE.WRENCH;

    protected override void Start()
    {
        base.Start();
    }

    public override void PerformAction(Agent agent)
    {
        if (agent.m_agentInventory.AgentHasItem(m_requiredItem))
        {
            foreach (EnviromentHazard enviromentalHazard in m_activatedHazard)
            {
                enviromentalHazard.ActivateHazard();
            }
        }
    }
}
