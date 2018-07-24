using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldState : ScriptableObject
{
    public enum WORLD_STATE { IDLE, PATROL, USING_NODE, AT_NODE, ATTACKING_TARGET, HAS_TARGET, HAS_LINE_OF_SIGHT, HAS_TARGET_NODE, CLOSE_TO_TARGET}
    public static Dictionary<WORLD_STATE, Func<GameObject, bool>> m_StateFunctions = new Dictionary<WORLD_STATE, Func<GameObject, bool>>
    {
        //{WORLD_STATE.IDLE, Idle}
        //{WORLD_STATE.CLOSE_TO_TARGET, CloseToTarget},
        //{WORLD_STATE.HAS_TARGET, HasTarget},
        {WORLD_STATE.HAS_TARGET_NODE, HasTargetNode}
        //{WORLD_STATE.AT_NODE, AtNode}
    };

    public static bool CheckForValidState(Agent p_agent, WORLD_STATE p_state)
    {
        if (m_StateFunctions.ContainsKey(p_state))
            return m_StateFunctions[p_state](p_agent.gameObject);
        return false;
    }

    //--------------------------------------------------------------------------------------
    // Determining if agent can be idle
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		always true, idle is defualt
    //--------------------------------------------------------------------------------------
    private static bool Idle(Agent p_agent)
    {
        return true;
    }

    //--------------------------------------------------------------------------------------
    // Get closest valid target to agent
    // Compare against range of agent
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		true when range is greater than distance
    //--------------------------------------------------------------------------------------
    //public static bool CloseToTarget(GameObject agent)
    //{
    //    Agent agentScript = agent.GetComponent<Agent>();
    //    NPCAgentPlanner planner = agent.GetComponent<NPCAgentPlanner>();
    //    if (agentScript != null && planner != null)
    //    {
    //        GameObject closestTarget = null;
    //        float dis = 0.0f;
    //        planner.GetClosesetTarget(ref closestTarget, ref dis);
    //        if (closestTarget && dis < agentScript.m_range)
    //            return true;
    //    }
    //    return false;
    //}

    //--------------------------------------------------------------------------------------
    // Check agents seen targets to see if theres any valid targets 
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		true when at least one target still is seen
    //--------------------------------------------------------------------------------------
    //public static bool HasTarget(GameObject agent)
    //{
    //    NPCAgentPlanner planner = agent.GetComponent<NPCAgentPlanner>();
    //    if (planner != null)
    //    {
    //        return planner.PossibleTargets.Count > 0;
    //    }
    //    return false;
    //}

    //--------------------------------------------------------------------------------------
    // Check agents seen targets to see if theres any valid targets 
     
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		true when at least one target still is seen
    //--------------------------------------------------------------------------------------
    public static bool HasTargetNode(GameObject agent)
    {
        NPC planner = agent.GetComponent<NPC>();
        if (planner != null)
        {
            return planner.m_agentState.m_targetNode != null;
        }
        return false;
    }

    //--------------------------------------------------------------------------------------
    // Determine if agent is at target node
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		true when the node below agent is equal to agents target node
    //--------------------------------------------------------------------------------------
    //public static bool AtNode(GameObject agent)
    //{
    //    Agent agentScript = agent.GetComponent<Agent>();
    //    NPCAgentPlanner planner = agent.GetComponent<NPCAgentPlanner>();
    //    if (agentScript != null && planner != null)
    //    {
    //        return agentScript.GetNodeBelow() == planner.TargetNode;
    //    }
    //    return false;
    //}
}
