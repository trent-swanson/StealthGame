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
    }

    public override void PerformAction(Agent agent)
    {
        if (m_usable && agent.m_agentInventory.AgentHasItem(m_requiredItem))
        {
            foreach (EnviromentHazard enviromentalHazard in m_activatedHazards)
            {
                enviromentalHazard.ActivateHazard();
            }
        }

        m_usable = false;
        //StartCoroutine(DisableHazard(m_steamEffect));
    }

    //private IEnumerator DisableHazard(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    foreach (EnviromentHazard enviromentalHazard in m_activatedHazards)
    //    {
    //        enviromentalHazard.DeactivateHazard();
    //    }
    //}
}


