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
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void ActionInit(Agent agent)
    {

    }

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public override bool IsDone(Agent agent)
    {
        return true;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(Agent agent)
    {

    }


    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void Perform(Agent agent)
    {
        NPC NPCScript = agent.GetComponent<NPC>();
        Goal goal = NPCScript.m_currentGoal;

        switch (goal.m_desiredWorldState)
        {
            case WorldState.WORLD_STATE.PATROL:
                NPCScript.m_agentState.m_targetNode = NPCScript.m_waypoints[NPCScript.m_currentWaypoint];
                break;
            case WorldState.WORLD_STATE.ATTACKING_TARGET:
                NPCScript.m_agentState.m_targetNode = NPCScript.m_agentState.m_possibleTargets[0]; // TODO get better target
                break;
            default:
                break;
        }
    }
}
