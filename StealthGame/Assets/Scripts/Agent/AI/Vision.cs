using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public static List<NavNode> BuildVisionList(Agent agent)
    {
        Agent.FACING_DIR facingDir = agent.m_facingDir ;
        int visionDistance = agent.m_visionDistance;
        float visionCone = agent.m_visionAngle;

        List<NavNode> visibleNodes = new List<NavNode>();

        List<NavNode> openNodes = new List<NavNode>();
        openNodes.Add(agent.m_currentNavNode);
        while (visionDistance > 0 && openNodes.Count > 0)
        {
            visibleNodes.AddRange(openNodes);

            openNodes = GetNextLayer(openNodes, facingDir, visionCone);

            visionDistance--; //Ends when vision is too far
        }

        return visibleNodes;
    }

    private static List<NavNode> GetNextLayer(List<NavNode> openNodes, Agent.FACING_DIR facingDir, float visionCone)
    {
        List<NavNode> newOpenNodes = new List<NavNode>();
        //Get forward node
        NavNode newNode = GetNavNode(openNodes[0], facingDir);
        if (newNode!= null && (newNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || newNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE))
            newOpenNodes.Add(newNode);
        //Get leftwards node

        //Get rightwards node

        return newOpenNodes;
    }

    private static NavNode GetNavNode(NavNode currentNavNode, Agent.FACING_DIR facingDir)
    {
        List<NavNode> possibleNodes = new List<NavNode>();
        
        switch (facingDir) //Get all navnodes adject to current
        {
            case Agent.FACING_DIR.NORTH:
                possibleNodes.AddRange(currentNavNode.m_northNodes);
                break;
            case Agent.FACING_DIR.EAST:
                possibleNodes.AddRange(currentNavNode.m_eastNodes);
                break;
            case Agent.FACING_DIR.SOUTH:
                possibleNodes.AddRange(currentNavNode.m_southNodes);
                break;
            case Agent.FACING_DIR.WEST:
                possibleNodes.AddRange(currentNavNode.m_westNodes);
                break;
            case Agent.FACING_DIR.NONE:
                break;
            default:
                break;
        }

        foreach (NavNode navNode in possibleNodes) //Only get nav nodes on same height
        {
            if (navNode.m_gridPos.y == currentNavNode.m_gridPos.y)
                return navNode;
        }

        return null;
    }
}
