using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetTargetNode", menuName = "AI Actions/GetTargetNode")]
public class GetTargetNode : AIAction
{
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
        return NPCAgent.m_agentWorldState.m_targetNode != null;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(NPC NPCAgent)
    {

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
        Goal goal = NPCAgent.m_agentWorldState.m_goal;

        switch (goal.m_desiredWorldState)
        {
            case WorldState.WORLD_STATE.PATROLLING:
                NPCAgent.m_agentWorldState.m_targetNode = NPCAgent.m_agentWorldState.m_waypoints[NPCAgent.m_agentWorldState.m_waypointIndex];
                break;
            default:
                break;
        }
    }
}
