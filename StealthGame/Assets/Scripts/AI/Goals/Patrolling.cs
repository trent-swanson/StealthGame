using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrolling", menuName = "AI Goals/Patrolling")]
public class Patrolling : Goal
{

    //--------------------------------------------------------------------------------------
    // Determining a goals priority
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		realtive priority based off agents's states, range from 0-1
    //      In future this is where squad manager can "buff" desiered goals
    //--------------------------------------------------------------------------------------
    public override float DetermineGoalPriority(GameObject agent)
    {
        return 0;
    }
}
