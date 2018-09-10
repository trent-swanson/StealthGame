using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldState : ScriptableObject
{
    public enum WORLD_STATE { IDLING, PATROLLING, USING_NODE, AT_NODE, HAS_TARGET_NODE, ATTACKING_TARGET, AT_TARGET, TARGET_IN_LINE_OF_SIGHT,
                                NEAR_TARGET, AT_LAST_TARGET_LOCATION, AT_INVESTIGATE_LOCATION, KO_GUARD_IN_LINE_OF_SIGHT, HELPING_GUARD,
                                SEARCHED_AREA, INVESTIGATED, GUARDING_TARGET, FOUND_TARGET, AMBUSHED_TARGET}
    public static Dictionary<WORLD_STATE, Func<NPC, bool>> m_StateFunctions = new Dictionary<WORLD_STATE, Func<NPC, bool>>
    {
        //{WORLD_STATE.IDLING, Idle}
        //{WORLD_STATE.CLOSE_TO_TARGET, CloseToTarget},
        //{WORLD_STATE.HAS_TARGET, HasTarget},
        {WORLD_STATE.HAS_TARGET_NODE, HasTargetNode},
        {WORLD_STATE.AT_NODE, AtNode},
        {WORLD_STATE.NEAR_TARGET, NearTarget}
    };

    public static bool CheckForValidState(NPC NPCAgent, WORLD_STATE p_state)
    {
        if (m_StateFunctions.ContainsKey(p_state))
            return m_StateFunctions[p_state](NPCAgent);
        return false;
    }

    //--------------------------------------------------------------------------------------
    // Determining if agent can be idle
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		always true, idle is defualt
    //--------------------------------------------------------------------------------------
    private static bool Idle(NPC NPCAgent)
    {
        return true;
    }

    //--------------------------------------------------------------------------------------
    // Get closest valid target to agent
    // Compare against range of agent
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when range is greater than distance
    //--------------------------------------------------------------------------------------
    //public static bool CloseToTarget(NPC NPCAgent)
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
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when at least one target still is seen
    //--------------------------------------------------------------------------------------
    //public static bool HasTarget(NPC NPCAgent)
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
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when at least one target still is seen
    //--------------------------------------------------------------------------------------
    public static bool HasTargetNode(NPC NPCAgent)
    {
        return NPCAgent.m_targetNode != null;
    }

    //--------------------------------------------------------------------------------------
    // Determine if agent is at target node
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when the node below agent is equal to agents target node
    //--------------------------------------------------------------------------------------
    public static bool AtNode(NPC NPCAgent)
    {
        if(NPCAgent.m_targetNode != null)
            return (NPCAgent.m_targetNode == NPCAgent.m_currentNavNode);
        return false;
    }

    //--------------------------------------------------------------------------------------
    // Determine if agent is near a target
    // Check against current nav node is one node away or is targetnode
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when the node below agent is equal to agents target node
    //--------------------------------------------------------------------------------------
    public static bool NearTarget(NPC NPCAgent)
    {
        if (NPCAgent.m_attackingTarget != null)
        {
            if (NPCAgent.m_attackingTarget.m_currentNavNode == NPCAgent.m_currentNavNode)//Current Node
                return true;
            if (NPCAgent.m_attackingTarget.m_currentNavNode.m_gridPos + new Vector3Int(0,0,1)  == NPCAgent.m_currentNavNode.m_gridPos)//forwards Node
                return true;
            if (NPCAgent.m_attackingTarget.m_currentNavNode.m_gridPos + new Vector3Int(0, 0, -1) == NPCAgent.m_currentNavNode.m_gridPos)//Backwards Node
                return true;
            if (NPCAgent.m_attackingTarget.m_currentNavNode.m_gridPos + new Vector3Int(1, 0, 0) == NPCAgent.m_currentNavNode.m_gridPos)//Right Node
                return true;
            if (NPCAgent.m_attackingTarget.m_currentNavNode.m_gridPos + new Vector3Int(-1, 0, 0) == NPCAgent.m_currentNavNode.m_gridPos)//Left Node
                return true;
        }
        return false;
    }
}
