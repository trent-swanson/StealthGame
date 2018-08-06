using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : ScriptableObject
{
    public static int m_outlineLayer = 0;
    public static int m_navNodeLayer = 0;
    public static int m_interactableLayer = 0;
    public static int m_enviromentLayer = 0;

    static LayerManager()
    {
        m_enviromentLayer = LayerMask.GetMask("Enviroment");
        m_outlineLayer = LayerMask.GetMask("Outline");
        m_navNodeLayer = LayerMask.GetMask("NavNode");
        m_interactableLayer = LayerMask.GetMask("Interactable");
    }

}
