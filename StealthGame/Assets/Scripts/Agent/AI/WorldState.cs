using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldState : ScriptableObject
{
    public enum WORLD_STATE { IDLING, PATROLLING, USING_NODE, AT_OBJECT_NODE, AT_PATROL_NODE, AT_INVESTIGATION_NODE, ATTACKING_TARGET, TARGET_IN_LINE_OF_SIGHT,
                                NEAR_TARGET, AT_INVESTIGATE_LOCATION, KO_GUARD_IN_LINE_OF_SIGHT, HELPING_GUARD,
                                SEARCHED_AREA, INVESTIGATED, GUARDING_TARGET, FOUND_TARGET, AMBUSHED_TARGET}
    public static Dictionary<WORLD_STATE, Func<NPC, bool>> m_StateFunctions = new Dictionary<WORLD_STATE, Func<NPC, bool>>
    {
        {WORLD_STATE.AT_OBJECT_NODE, AtObjectNode},
        {WORLD_STATE.AT_PATROL_NODE, AtPatrolNode},
        {WORLD_STATE.AT_INVESTIGATION_NODE, AtInvestigationNode},
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
    // Determine if agent is at target node
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when the node below agent is equal to agents target node
    //--------------------------------------------------------------------------------------
    public static bool AtObjectNode(NPC NPCAgent)
    {
        //TODO
        return false;
    }

    //--------------------------------------------------------------------------------------
    // Determine if agent is at target node
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when the node below agent is equal to agents target node
    //--------------------------------------------------------------------------------------
    public static bool AtPatrolNode(NPC NPCAgent)
    {
        if (NPCAgent.m_agentWorldState.m_waypoints.Count > 0)
            return (NPCAgent.m_agentWorldState.m_waypoints[NPCAgent.m_agentWorldState.m_waypointIndex] == NPCAgent.m_currentNavNode);
        return false;
    }

    //--------------------------------------------------------------------------------------
    // Determine if agent is at target node
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		true when the node below agent is equal to agents target node
    //--------------------------------------------------------------------------------------
    public static bool AtInvestigationNode(NPC NPCAgent)
    {
        List<NPC.InvestigationNode> m_investigationNodes = NPCAgent.m_agentWorldState.GetInvestigationNodes();
        if (m_investigationNodes.Count > 0)
            return (m_investigationNodes[0].m_node == NPCAgent.m_currentNavNode);
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
        if (NPCAgent.m_targetAgent != null)
        {
            if (NPCAgent.m_targetAgent.m_currentNavNode == NPCAgent.m_currentNavNode)//Current Node
                return true;
            if (NPCAgent.m_targetAgent.m_currentNavNode.m_gridPos + new Vector3Int(0,0,1)  == NPCAgent.m_currentNavNode.m_gridPos)//forwards Node
                return true;
            if (NPCAgent.m_targetAgent.m_currentNavNode.m_gridPos + new Vector3Int(0, 0, -1) == NPCAgent.m_currentNavNode.m_gridPos)//Backwards Node
                return true;
            if (NPCAgent.m_targetAgent.m_currentNavNode.m_gridPos + new Vector3Int(1, 0, 0) == NPCAgent.m_currentNavNode.m_gridPos)//Right Node
                return true;
            if (NPCAgent.m_targetAgent.m_currentNavNode.m_gridPos + new Vector3Int(-1, 0, 0) == NPCAgent.m_currentNavNode.m_gridPos)//Left Node
                return true;
        }
        return false;
    }
}
