using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeValve : Interactable
{
    public List<EnviromentHazard> m_activatedHazards = new List<EnviromentHazard>();

    public float m_steamEffect = 2.0f;

    //--------------------------------------------------------------------------------------
    // Initialisation
    // Ensure interactable has a conected canvas
    // Set navigation nodes to be a item based interatable 
    //
    // Set up visual ques 
    //--------------------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        FadeCanvas();
    }

    //--------------------------------------------------------------------------------------
    // Can agent use this interactable
    // Requires agent to have the required item
    //
    // Param
    //		agent: agent to check condition against
    // Return:
    //      bool when usable
    //--------------------------------------------------------------------------------------
    public override bool CanPerform(Agent agent)
    {
        return m_usable && agent.m_agentInventory.AgentHasItem(m_requiredItem);
    }

    //--------------------------------------------------------------------------------------
    // Perform interactables actions
    // Activate connected hazard
    //
    // Param
    //		agent: agent to perform action
    //--------------------------------------------------------------------------------------
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


