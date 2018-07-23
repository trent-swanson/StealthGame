using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal : ScriptableObject
{
    [Tooltip("What state this goal will achieve")]
    [SerializeField]
    public WorldState.WORLD_STATE m_desiredWorldState = WorldState.WORLD_STATE.IDLE;
    
    public int m_goalPriority;

    //--------------------------------------------------------------------------------------
    // Determining a goals priority
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		realtive priority based off agents's states, range from 0-1
    //      In future this is where squad manager can "buff" desiered goals
    //--------------------------------------------------------------------------------------
    public abstract float DetermineGoalPriority(GameObject agent); 
}
