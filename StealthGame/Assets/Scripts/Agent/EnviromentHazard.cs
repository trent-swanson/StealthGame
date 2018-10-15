using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentHazard : MonoBehaviour
{
    public List<NavNode> m_effectedAreas = new List<NavNode>();

    public virtual void ActivateHazard()
    {

    }

    public virtual void DeactivateHazard()
    {

    }
}
