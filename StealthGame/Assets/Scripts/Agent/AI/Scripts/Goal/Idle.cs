using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "AI Goals/Idle")]
public class Idle : Goal
{
    //--------------------------------------------------------------------------------------
    // Determining a goals priority
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void DetermineGoalPriority(NPC NPCAgent)
    {
        m_goalPriority = m_defualtPriority;
    }
}