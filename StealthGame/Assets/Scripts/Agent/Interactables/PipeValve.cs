using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeValve : Interactable
{
    public EnviromentHazard m_activatedHazard = null;

    protected override void Start()
    {
        base.Start();
    }

    public override void PerformAction(Agent agent)
    {
        m_activatedHazard.ActivateHazard();
    }
}
