using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeValve : Interactable
{
    public List<EnviromentHazard> m_activatedHazards = new List<EnviromentHazard>();

    public float m_steamEffect = 2.0f;

    protected override void Start()
    {
        base.Start();

        FadeCanvas();
    }

    public override bool CanPerform(Agent agent)
    {
        return m_usable && agent.m_agentInventory.AgentHasItem(m_requiredItem);
    }

    public override void PerformAction(Agent agent)
    {
        base.PerformAction(agent);

        foreach (EnviromentHazard enviromentalHazard in m_activatedHazards)
        {
            enviromentalHazard.ActivateHazard();
        }

        m_usable = false;
    }
}


