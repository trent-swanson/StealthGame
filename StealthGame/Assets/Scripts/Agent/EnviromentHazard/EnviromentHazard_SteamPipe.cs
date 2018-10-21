using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentHazard_SteamPipe : EnviromentHazard
{
    public GameObject m_activatedSteamObj = null;

    public override void ActivateHazard()
    {
        m_activatedSteamObj.SetActive(true);

        foreach (NavNode effectedNavNode in m_effectedAreas)
        {
            if (effectedNavNode.m_obstructingAgent != null)
                effectedNavNode.m_obstructingAgent.Knockout();
        }
    }
}
