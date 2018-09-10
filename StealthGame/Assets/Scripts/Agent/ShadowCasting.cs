using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCasting : MonoBehaviour
{
    public static void BuildVision(NPC npc)
    {
        int visionDistance = npc.m_visionDistance;
        float visionAngle = npc.m_visionAngle;

        //Build list of tiles within range and within vision cone

        //Build shadow casting into list -> Removes those which are blocked

        //Updated final nodes with the guards who can see them


    }
}
