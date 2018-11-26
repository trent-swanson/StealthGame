using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    //--------------------------------------------------------------------------------------
    // Build a NPC vision cone
    // Basic vision setup, based off direction/distance, move to next line of possible nodes
    // Check if within vision cone angle and close enough, if so add to vision 
    // Finally at end, raycast back for each node to ensure possible vision. Stop vision bending around corners
    // 
    // Param
    //		agent: agenet to start at
    //		visionDistance: how far to travel in units. One tile is currently 2 units
    //		visionCone: angle to base building of nodes off
    //		playerTurn: On player turn, start at agent and adjacent left/right nodes. Otherwise just current node
    // Return:
    //      List of navnodes within vision
    //--------------------------------------------------------------------------------------
    public static List<NavNode> BuildVisionList(Agent agent, float visionDistance, float visionCone, bool playerTurn)
    {
        FACING_DIR facingDir = agent.m_facingDir ;

        NavNode startingNode = agent.m_currentNavNode;

        List<NavNode> visibleNodes = new List<NavNode>();

        List<NavNode> openNodes = new List<NavNode>();

        openNodes.Add(startingNode);

        //TODO Use different vision system

        while (openNodes.Count > 0)
        {
            visibleNodes.AddRange(openNodes);

            openNodes = GetNextLayer(agent, startingNode, openNodes, facingDir, visionDistance, visionCone);
        }

        //Add left right nodes when Player turn 
        if(playerTurn)
        {
            FACING_DIR rightDir = GetRelativeDir(facingDir, LEFT_RIGHT.RIGHT);
            NavNode rightNode = startingNode.GetAdjacentNode(rightDir);
            if (rightNode != null && (rightNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || rightNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || rightNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE))
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
            if (leftNode != null && (leftNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || leftNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || leftNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE))
            {
                openNodes.Add(leftNode);
                while (openNodes.Count > 0)
                {
                    visibleNodes.AddRange(openNodes);

                    openNodes = GetNextLayer(agent, leftNode, openNodes, facingDir, visionDistance, visionCone);
                }
            }
        }

        Vector3 heightOffset = new Vector3(0.0f, 0.2f, 0.0f); 
        Vector3 startingPos = agent.m_currentNavNode.m_nodeTop + heightOffset;
        for (int i = 0; i < visibleNodes.Count; i++)
        {
            Vector3 dir = visibleNodes[i].m_nodeTop + heightOffset - startingPos;//Get dir between node tops

            RaycastHit hit;
            if (Physics.Raycast(startingPos, dir, out hit, dir.magnitude, LayerManager.m_enviromentLayer))
            {
                visibleNodes.RemoveAt(i);
                i--;
            }
        }

        return visibleNodes;
    }

    //--------------------------------------------------------------------------------------
    // Get next layer of nav nodes
    // Potential nav nodes are wihtin distance, wihtin vision cone from previous node, and in states obstructed/walkable/interatacble 
    //
    // Param
    //		agent: agenet to start at
    //		startingNode: node to base distance/angle from
    //		openNodes: all nodes that are valid vions nodes
    //		facingDir: direction to base forward direction as, this determins the angle
    //		visionDistance: how far to travel in units. One tile is currently 2 units
    //		visionCone: angle to base building of nodes off
    // Return:
    //      List of navnodes within vision
    //--------------------------------------------------------------------------------------
    private static List<NavNode> GetNextLayer(Agent agent, NavNode startingNode, List<NavNode> openNodes, FACING_DIR facingDir, float visionDistance, float visionCone)
    {
        List<NavNode> newOpenNodes = new List<NavNode>();
        List<NavNode> forwardNodes = new List<NavNode>();

        foreach (NavNode navNode in openNodes)
        {
            //Get forward node
            NavNode forwardNode = GetNavNode(agent, startingNode, navNode, facingDir, visionDistance, visionCone);
            if (forwardNode != null && (forwardNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || forwardNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || forwardNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE) && !newOpenNodes.Contains(forwardNode))
            {
                forwardNodes.Add(forwardNode);
            }
        }

        newOpenNodes.AddRange(forwardNodes);

        foreach (NavNode forwardNode in forwardNodes)
        {
            //Get rightwards node
            NavNode leftNode = GetNavNode(agent, startingNode, forwardNode, GetRelativeDir(facingDir, LEFT_RIGHT.RIGHT), visionDistance, visionCone);
            if (leftNode != null && (leftNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || leftNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || leftNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE) && !newOpenNodes.Contains(leftNode))
                newOpenNodes.Add(leftNode);

            //Get leftwards node
            NavNode rightNode = GetNavNode(agent, startingNode, forwardNode, GetRelativeDir(facingDir, LEFT_RIGHT.LEFT), visionDistance, visionCone);
            if (rightNode != null && (rightNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || rightNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || rightNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE) && !newOpenNodes.Contains(rightNode))
                newOpenNodes.Add(rightNode);
        }

        return newOpenNodes;
    }

    //--------------------------------------------------------------------------------------
    // Get potential nav node. Conditions are wihtin distance, wihtin vision cone from previous node, and in states obstructed/walkable/interatacble 
    //
    // Param
    //		agent: agenet to start at
    //		startingNode: node to base distance/angle from
    //		facingDir: direction to base forward direction as, this determins the angle
    //		visionDistance: how far to travel in units. One tile is currently 2 units
    //		visionCone: angle to base building of nodes off
    // Return:
    //      List of navnode, if null, node is not valid
    //--------------------------------------------------------------------------------------
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

    //--------------------------------------------------------------------------------------
    // Get next direction based of a forwards direction
    //
    // Param
    //		facingDir: Forwards direction to be checked against
    //		relativeDir: given forwards dir, get left or right relative direction
    // Return:
    //      relative direction
    //--------------------------------------------------------------------------------------
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
