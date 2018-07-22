using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetPatrolNode", menuName = "AIBehaviours/Actions/Get Patrol Node")]
public class GetPatrolNodeAction : Action
{
    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void ActionInit(GameObject agent)
    {
        NPCAgentPlanner NPCPlannerScript = agent.GetComponent<NPCAgentPlanner>();

        if (NPCPlannerScript != null)
        {
            int waypointIndex = NPCPlannerScript.WaypointIndex;
            waypointIndex++;
            if (waypointIndex == NPCPlannerScript.Waypoints.Count)
                waypointIndex = 0;
            NPCPlannerScript.WaypointIndex = waypointIndex;

            NPCPlannerScript.TargetNode = NPCPlannerScript.Waypoints[waypointIndex];
        }
    }

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public override bool IsDone(GameObject agent)
    {
        return true;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(GameObject agent)
    {
    }

    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void Perform(GameObject agent)
    {

    }
}
