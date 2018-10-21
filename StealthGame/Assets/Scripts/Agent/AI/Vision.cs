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

        //TODO make better

        while (openNodes.Count > 0)
        {
            visibleNodes.AddRange(openNodes);

            openNodes = GetNextLayer(agent, startingNode, openNodes, facingDir, visionDistance, visionCone);
        }

        //Add left right nodes
        FACING_DIR rightDir = GetRelativeDir(facingDir, LEFT_RIGHT.RIGHT);
        NavNode rightNode = startingNode.GetAdjacentNode(rightDir);
        if (rightNode != null && (rightNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || rightNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE))
        {
            openNodes.Add(rightNode);
            while (openNodes.Count > 0)
            {
                visibleNodes.AddRange(openNodes);

                openNodes = GetNextLayer(agent, rightNode, openNodes, facingDir, visionDistance, visionCone);
            }
        }

        FACING_DIR leftDir = GetRelativeDir(facingDir, LEFT_RIGHT.LEFT);
        NavNode leftNode = startingNode.GetAdjacentNode(leftDir);
        if (leftNode != null && (leftNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || leftNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE))
        {
            openNodes.Add(leftNode);
            while (openNodes.Count > 0)
            {
                visibleNodes.AddRange(openNodes);

                openNodes = GetNextLayer(agent, leftNode, openNodes, facingDir, visionDistance, visionCone);
            }
        }



        return visibleNodes;
    }

    private static List<NavNode> GetNextLayer(Agent agent, NavNode startingNode, List<NavNode> openNodes, FACING_DIR facingDir, float visionDistance, float visionCone)
    {
        List<NavNode> newOpenNodes = new List<NavNode>();
        List<NavNode> forwardNodes = new List<NavNode>();

        foreach (NavNode navNode in openNodes)
        {
            //Get forward node
            NavNode forwardNode = GetNavNode(agent, startingNode, navNode, facingDir, visionDistance, visionCone);
            if (forwardNode != null && (forwardNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || forwardNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE) && !newOpenNodes.Contains(forwardNode))
            {
                forwardNodes.Add(forwardNode);
            }
        }

        newOpenNodes.AddRange(forwardNodes);

        foreach (NavNode forwardNode in forwardNodes)
        {
            //Get rightwards node
            NavNode leftNode = GetNavNode(agent, startingNode, forwardNode, GetRelativeDir(facingDir, LEFT_RIGHT.RIGHT), visionDistance, visionCone);
            if (leftNode != null && (leftNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || leftNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE) && !newOpenNodes.Contains(leftNode))
                newOpenNodes.Add(leftNode);

            //Get leftwards node
            NavNode rightNode = GetNavNode(agent, startingNode, forwardNode, GetRelativeDir(facingDir, LEFT_RIGHT.LEFT), visionDistance, visionCone);
            if (rightNode != null && (rightNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || rightNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE) && !newOpenNodes.Contains(rightNode))
                newOpenNodes.Add(rightNode);
        }

        return newOpenNodes;
    }

    private static NavNode GetNavNode(Agent agent, NavNode startingNode, NavNode currentNavNode, FACING_DIR facingDir, float visionDistance, float visionCone)
    {
        List<NavNode> possibleNodes = new List<NavNode>();

        NavNode nextNavNode = null;

        nextNavNode = currentNavNode.GetAdjacentNode(facingDir);

        if (nextNavNode == null)
            return null;

        //Distance
        if (Vector3.Distance(nextNavNode.m_nodeTop, startingNode.m_nodeTop) > visionDistance)
            return null;

        //Get navNode within vision cone
        Vector3 startToNode = (nextNavNode.m_nodeTop - startingNode.m_nodeTop).normalized;
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
