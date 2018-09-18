using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public static List<NavNode> BuildVisionList(Agent agent, float visionDistance, float visionCone)
    {
        FACING_DIR facingDir = agent.m_facingDir ;

        NavNode startingNode = agent.m_currentNavNode;

        List<NavNode> visibleNodes = new List<NavNode>();

        List<NavNode> openNodes = new List<NavNode>();
        openNodes.Add(startingNode);
        while (openNodes.Count > 0)
        {
            visibleNodes.AddRange(openNodes);

            openNodes = GetNextLayer(agent, openNodes, facingDir, visionDistance, visionCone);
        }

        return visibleNodes;
    }

    private static List<NavNode> GetNextLayer(Agent agent, List<NavNode> openNodes, FACING_DIR facingDir, float visionDistance, float visionCone)
    {
        List<NavNode> newOpenNodes = new List<NavNode>();
        List<NavNode> forwardNodes = new List<NavNode>();

        foreach (NavNode navNode in openNodes)
        {
            //Get forward node
            NavNode forwardNode = GetNavNode(agent, navNode, facingDir, visionDistance, visionCone);
            if (forwardNode != null && (forwardNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || forwardNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE) && !newOpenNodes.Contains(forwardNode))
            {
                forwardNodes.Add(forwardNode);
            }
        }

        newOpenNodes.AddRange(forwardNodes);

        foreach (NavNode forwardNode in forwardNodes)
        {
            //Get rightwards node
            NavNode leftNode = GetNavNode(agent, forwardNode, GetRelativeDir(facingDir, LEFT_RIGHT.RIGHT), visionDistance, visionCone);
            if (leftNode != null && (leftNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || leftNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE) && !newOpenNodes.Contains(leftNode))
                newOpenNodes.Add(leftNode);

            //Get leftwards node
            NavNode rightNode = GetNavNode(agent, forwardNode, GetRelativeDir(facingDir, LEFT_RIGHT.LEFT), visionDistance, visionCone);
            if (rightNode != null && (rightNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || rightNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE) && !newOpenNodes.Contains(rightNode))
                newOpenNodes.Add(rightNode);
        }

        return newOpenNodes;
    }

    private static NavNode GetNavNode(Agent agent, NavNode currentNavNode, FACING_DIR facingDir, float visionDistance, float visionCone)
    {
        List<NavNode> possibleNodes = new List<NavNode>();

        NavNode nextNavNode = null;

        switch (facingDir) //Get all navnodes adject to current
        {
            case FACING_DIR.NORTH:
                possibleNodes.AddRange(currentNavNode.m_northNodes);
                break;
            case FACING_DIR.EAST:
                possibleNodes.AddRange(currentNavNode.m_eastNodes);
                break;
            case FACING_DIR.SOUTH:
                possibleNodes.AddRange(currentNavNode.m_southNodes);
                break;
            case FACING_DIR.WEST:
                possibleNodes.AddRange(currentNavNode.m_westNodes);
                break;
            case FACING_DIR.NONE:
                break;
            default:
                break;
        }

        //Only get nav nodes on same height
        foreach (NavNode navNode in possibleNodes) 
        {
            if (navNode.m_gridPos.y == currentNavNode.m_gridPos.y)
                nextNavNode = navNode;
        }

        if (nextNavNode == null)
            return null;

        //Distance
        if (Vector3.Distance(nextNavNode.m_nodeTop, agent.m_currentNavNode.m_nodeTop) > visionDistance)
            return null;

        //Get navNode within vision cone
        Vector3 startToNode = (nextNavNode.m_nodeTop - agent.m_currentNavNode.m_nodeTop).normalized;
        Vector3 facingVector = Agent.FacingDirEnumToVector3(agent.m_facingDir);

        float angle = Mathf.Acos(Vector3.Dot(startToNode, facingVector));

        if(angle * Mathf.Rad2Deg <= visionCone / 2)
            return nextNavNode;
        return null;
    }

    private enum LEFT_RIGHT {RIGHT = 1, LEFT = -1 }
    private static FACING_DIR GetRelativeDir(FACING_DIR facingDir, LEFT_RIGHT relativeDir)
    {
        int dirAmount = (int)relativeDir + (int)facingDir;

        if (dirAmount == 4)
            dirAmount = 0;
        else if (dirAmount == -1)
            dirAmount = 3;

        return (FACING_DIR)dirAmount;
    }
}
