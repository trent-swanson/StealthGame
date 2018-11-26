using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentHazard : MonoBehaviour
{
    public List<NavNode> m_effectedAreas = new List<NavNode>();

    //--------------------------------------------------------------------------------------
    // Hazard is activated
    //--------------------------------------------------------------------------------------
    public virtual void ActivateHazard()
    {

    }

    //--------------------------------------------------------------------------------------
    // Hazard is deactivated
    //--------------------------------------------------------------------------------------
    public virtual void DeactivateHazard()
    {

    }
}
