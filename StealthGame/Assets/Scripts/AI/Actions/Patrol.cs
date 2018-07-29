using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol", menuName = "AI Actions/Patrol")]
public class Patrol : AIAction {

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void ActionInit(NPC NPCAgent)
    {

    }

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public override bool IsDone(NPC NPCAgent)
    {
        return (NPCAgent.transform.position == NPCAgent.m_agentWorldState.m_waypoints[NPCAgent.m_agentWorldState.m_waypointIndex].transform.position);
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(NPC NPCAgent)
    {
        NPCAgent.m_agentWorldState.m_waypointIndex++;
        if (NPCAgent.m_agentWorldState.m_waypointIndex >= NPCAgent.m_agentWorldState.m_waypoints.Count)
            NPCAgent.m_agentWorldState.m_waypointIndex = 0;

        NPCAgent.m_agentWorldState.m_targetNode = null;
    }


    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void Perform(NPC NPCAgent)
    {

    }
}
