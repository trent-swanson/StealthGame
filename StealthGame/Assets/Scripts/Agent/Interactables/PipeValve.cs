using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeValve : Interactable
{
    protected override void Start()
    {
        base.Start();
    }

    public override void PerformAction(Agent agent)
    {
        Debug.Log("Busting Pipe");
    }
}
