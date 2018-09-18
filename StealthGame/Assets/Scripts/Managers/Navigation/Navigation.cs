using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    private int m_maxLevelSize = 50;
    private int m_minYPos = -100;

    [Tooltip("How big a tile is in world units")]
    public Vector3 m_tileSize = new Vector3(2.0f, 1.0f, 2.0f);

    //Node type determination
    public static float m_lowObstacleHeight = 1.0f;
    public static float m_obstacleDetection = 1.0f;

    private int m_navNodeLayer = 0;

    //Caching of offsets for efficeincy
    public static Vector3 m_forwardOffset;
    public static Vector3 m_backwardOffset;
    public static Vector3 m_rightOffset;
    public static Vector3 m_leftOffset;

    //Storage of grid extents
    private int m_maxX = 0;
    private int m_minX = 0;
    private int m_maxY = 0;
    private int m_minY = 0;
    private int m_maxZ = 0;
    private int m_minZ = 0;


    public NavNode[,,] m_navGrid;
    public int m_navGridWidth;
    public int m_navGridDepth;
    public int m_navGridHeight;

    private void Awake()
    {
        //Setup stuff
        m_navNodeLayer = LayerMask.GetMask("NavNode");

        m_forwardOffset = new Vector3(0, 0, m_tileSize.z);
        m_backwardOffset = new Vector3(0, 0, -m_tileSize.z);
        m_rightOffset = new Vector3(m_tileSize.x, 0, 0);
        m_leftOffset = new Vector3(-m_tileSize.x, 0, 0);

        Vector3 orginatingPos = new Vector3(0.5f, m_minYPos, 0.5f);

        NavNode[,,] tempNavNodeGrid = new NavNode[m_maxLevelSize, m_maxLevelSize, m_maxLevelSize];
        Vector3Int gridCenter = new Vector3Int(m_maxLevelSize / 2, m_maxLevelSize / 2, m_maxLevelSize / 2);

        m_maxX = m_minX = gridCenter.x;
        m_maxY = m_minY = gridCenter.y;
        m_maxZ = m_minZ = gridCenter.z;


        List<NavNode> newNodes = GetNavNode(orginatingPos, gridCenter, tempNavNodeGrid);
        
        foreach (NavNode navNode in newNodes)
        {
            UpdateNeighbourNodes(navNode, tempNavNodeGrid);
        }

        //Create new grid clipping excess null filled sides
        m_navGridWidth = m_maxX - m_minX + 1;
        m_navGridHeight = m_maxY - m_minY + 1;
        m_navGridDepth = m_maxZ - m_minZ + 1;

        m_navGrid = new NavNode[m_navGridWidth, m_navGridHeight, m_navGridDepth];

        for (int i = 0; i < m_navGridWidth; i++)
        {
            for (int j = 0; j < m_navGridHeight; j++)
            {
                for (int k = 0; k < m_navGridDepth; k++)
                {
                    m_navGrid[i, j, k] = tempNavNodeGrid[m_minX + i, m_minY + j, m_minZ + k];
                    if (m_navGrid[i, j, k] != null)
                        m_navGrid[i, j, k].m_gridPos = new Vector3Int(i, j, k);
                }
            }
        }

        //Setup Node neighbours/Node type
        for (int i = 0; i < m_navGridWidth; i++)
        {
            for (int j = 0; j < m_navGridHeight; j++)
            {
                for (int k = 0; k < m_navGridDepth; k++)
                {
                    BuildNodeBranches(m_navGrid[i, j, k]);
                    m_navGrid[i, j, k].SetupNodeType();
                }
            }
        }

        //Setup Node wall hides TODO if dynamic enviroments move this into node on Node update
        for (int i = 0; i < m_navGridWidth; i++)
        {
            for (int j = 0; j < m_navGridHeight; j++)
            {
                for (int k = 0; k < m_navGridDepth; k++)
                {
                    NavNode navNode = m_navGrid[i, j, k];
                    if(navNode!=null)
                        m_navGrid[i, j, k].SetupWallHideIndicators(this);
                }
            }
        }
    }
    
    private List<NavNode> GetNavNode(Vector3 pos, Vector3Int gridPos, NavNode[,,] navNodeGrid)
    {
        List<NavNode> newNodes = new List<NavNode>();
        if (!IsGridColumnEmpty(gridPos.x, gridPos.z, navNodeGrid))//Case of a node already exisitng
            return newNodes;

        RaycastHit[] hits = Physics.RaycastAll(pos, Vector3.up, Mathf.Infinity, m_navNodeLayer);
        if (hits.Length != 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                NavNode navNode = hits[i].collider.GetComponent<NavNode>();

                if (navNode != null)
                {
                    NavNode originNode = navNodeGrid[m_maxLevelSize / 2, m_maxLevelSize / 2, m_maxLevelSize / 2];
                    if (originNode != null)//Get offset height
                    {
                        int heightDifference = Mathf.FloorToInt(navNode.transform.position.y - originNode.transform.position.y);

                        Vector3Int tempGridPos = gridPos;
                        tempGridPos.y = originNode.m_gridPos.y + heightDifference;
                        navNode.m_gridPos = tempGridPos;
                    }
                    else
                    {
                        navNode.m_gridPos = gridPos;
                    }

                    navNodeGrid[navNode.m_gridPos.x, navNode.m_gridPos.y, navNode.m_gridPos.z] = navNode;

                    m_minX = navNode.m_gridPos.x < m_minX ? navNode.m_gridPos.x : m_minX;
                    m_maxX = navNode.m_gridPos.x > m_maxX ? navNode.m_gridPos.x : m_maxX;
                    m_minY = navNode.m_gridPos.y < m_minY ? navNode.m_gridPos.y : m_minY;
                    m_maxY = navNode.m_gridPos.y > m_maxY ? navNode.m_gridPos.y : m_maxY;
                    m_minZ = navNode.m_gridPos.z < m_minZ ? navNode.m_gridPos.z : m_minZ;
                    m_maxZ = navNode.m_gridPos.z > m_maxZ ? navNode.m_gridPos.z : m_maxZ;

                    newNodes.Add(navNode);
                }
            }
        }
        return newNodes;
    }

    private bool IsGridColumnEmpty(int x, int z, NavNode[,,] navNodeGrid)
    {
        for (int i = 0; i < m_maxLevelSize; i++)
        {
            if (navNodeGrid[x, i, z] != null)
                return false;
        }
        return true;
    }

    //----------------
    //Creation of Nav Grid
    //----------------
    private void UpdateNeighbourNodes(NavNode currentNode, NavNode[,,] navNodeGrid)
    {
        Vector3 initialRaycastPos = currentNode.transform.position;
        initialRaycastPos.y = m_minYPos;

        List<NavNode> neightbourNodes = new List<NavNode>();

        //North
        neightbourNodes.AddRange(GetNavNode(initialRaycastPos + m_forwardOffset, currentNode.m_gridPos + new Vector3Int(0, 0, 1), navNodeGrid));

        //East
        neightbourNodes.AddRange(GetNavNode(initialRaycastPos + m_rightOffset, currentNode.m_gridPos + new Vector3Int(1, 0, 0), navNodeGrid));

        //South
        neightbourNodes.AddRange(GetNavNode(initialRaycastPos + m_backwardOffset, currentNode.m_gridPos + new Vector3Int(0, 0, -1), navNodeGrid));

        //West
        neightbourNodes.AddRange(GetNavNode(initialRaycastPos + m_leftOffset, currentNode.m_gridPos + new Vector3Int(-1, 0, 0), navNodeGrid));

        //Store all nodes in a single list for later usage

        foreach (NavNode navNode in neightbourNodes)
        {
            UpdateNeighbourNodes(navNode, navNodeGrid);
        }
    }

    private void BuildNodeBranches(NavNode currentNode)
    {
      
        if(currentNode!=null)
        {
            Vector3Int currentGridPos = currentNode.m_gridPos;

            //Forward
            if (currentGridPos.z + 1 < m_navGridDepth)
            {
                currentNode.m_northNodes.AddRange(GetAdjacentNode(currentGridPos + new Vector3Int(0, 0, 1)));
            }
            //Backward
            if (currentGridPos.z - 1 >= 0)
            {
                currentNode.m_southNodes.AddRange(GetAdjacentNode(currentGridPos + new Vector3Int(0, 0, -1)));
            }
            //Right
            if (currentGridPos.x + 1 < m_navGridWidth)
            {
                currentNode.m_eastNodes.AddRange(GetAdjacentNode(currentGridPos + new Vector3Int(1, 0, 0)));
            }
            //Left
            if (currentGridPos.x - 1 >= 0)
            {
                currentNode.m_westNodes.AddRange(GetAdjacentNode(currentGridPos + new Vector3Int(-1, 0, 0)));
            }

            currentNode.m_adjacentNodes.AddRange(currentNode.m_northNodes);
            currentNode.m_adjacentNodes.AddRange(currentNode.m_eastNodes);
            currentNode.m_adjacentNodes.AddRange(currentNode.m_southNodes);
            currentNode.m_adjacentNodes.AddRange(currentNode.m_westNodes);
        }
    }

    private List<NavNode> GetAdjacentNode(Vector3Int offsetGridPos)
    {
        List<NavNode> nodes = new List<NavNode>();
        //Mid
        if (m_navGrid[offsetGridPos.x, offsetGridPos.y, offsetGridPos.z] != null)
            nodes.Add(m_navGrid[offsetGridPos.x, offsetGridPos.y, offsetGridPos.z]);
        //Top
        if (offsetGridPos.y + 1 < m_navGridHeight && m_navGrid[offsetGridPos.x, offsetGridPos.y+1, offsetGridPos.z] != null)
            nodes.Add(m_navGrid[offsetGridPos.x, offsetGridPos.y+1, offsetGridPos.z]);
        //Lower
        if (offsetGridPos.y - 1 >= 0 && m_navGrid[offsetGridPos.x, offsetGridPos.y - 1, offsetGridPos.z] != null)
            nodes.Add(m_navGrid[offsetGridPos.x, offsetGridPos.y - 1, offsetGridPos.z]);

        return nodes;
    }

    //----------------
    //End of Nav Grid Creation
    //----------------

    //----------------
    //A* stuff
    //----------------
    public static List<NavNode> GetNavPath(NavNode startingNode, NavNode goalNode, Agent agent)
    {
        if (startingNode == goalNode)//Already at position
            return new List<NavNode>();

        List<NavNode> openNodes = new List<NavNode>();
        List<NavNode> closedNodes = new List<NavNode>();

        //Get starting node
        openNodes.Add(startingNode);

        NavNode currentNode = startingNode;
        currentNode.m_gScore = 0;//Reset starting node

        //Loop till no more options
        while (openNodes.Count > 0)
        {
            //Break early when at end
            if (currentNode == goalNode)
                return GetPath(currentNode, startingNode);

            AddNextNodes(currentNode, goalNode, openNodes, closedNodes, agent);

            currentNode = GetLowestFScore(openNodes);
        }
        return new List<NavNode>();
    }

    private static void AddNextNodes(NavNode currentNode, NavNode goalNode, List<NavNode> openNodes, List<NavNode> closedNodes, Agent agent)
    {
        openNodes.Remove(currentNode);
        closedNodes.Add(currentNode);

        foreach (NavNode nextNode in currentNode.m_adjacentNodes)
        {
            //Only add nodes which have not already been considered, are walkable and not already obstructed, unless it is obstructed by an enemy as attacking should take place.
            if (!openNodes.Contains(nextNode) && !closedNodes.Contains(nextNode) && 
                nextNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || nextNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE ||
                (nextNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED && nextNode.m_obstructingAgent != null && nextNode.m_obstructingAgent.m_team != agent.m_team))
            {
                openNodes.Add(nextNode);
                nextNode.Setup(openNodes, closedNodes, goalNode);
            }
        }
    }

    private static NavNode GetLowestFScore(List<NavNode> openNodes)
    {
        float fScore = Mathf.Infinity;
        NavNode highestFNode = null;
        foreach (NavNode node in openNodes)
        {
            if (node.m_fScore < fScore)
            {
                highestFNode = node;
                fScore = node.m_fScore;
            }
        }
        return highestFNode;
    }

    private static List<NavNode> GetPath(NavNode currentNode, NavNode startingNode)
    {
        List<NavNode> path = new List<NavNode>();

        while (currentNode != startingNode)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.m_previousNode;
        }
        path.Insert(0, currentNode);
        return path;
    }

    public NavNode.NODE_TYPE GetAdjacentNodeType(Vector3Int gridPos, Vector3Int gridOffset)
    {
        //Ensure offset is within range
        if(gridPos.x + gridOffset.x < 0 || gridPos.x + gridOffset.x > m_navGridWidth -1 || gridPos.y + gridOffset.y < 0 || gridPos.y + gridOffset.y > m_navGridHeight - 1 || gridPos.z + gridOffset.z < 0 || gridPos.z + gridOffset.z > m_navGridDepth - 1)
            return NavNode.NODE_TYPE.NONE;

        NavNode navNode = m_navGrid[gridPos.x + gridOffset.x, gridPos.y + gridOffset.y, gridPos.z + gridOffset.z]; //Get normal offest
        if(navNode != null)
            return navNode.m_nodeType;
        else
        {
            if (gridPos.y + gridOffset.y + 1 > m_navGridHeight - 1)//Check for step up
                return NavNode.NODE_TYPE.NONE;
            navNode = m_navGrid[gridPos.x + gridOffset.x, gridPos.y + gridOffset.y + 1, gridPos.z + gridOffset.z];//Get normal offset up one
            if (navNode != null)
                return NavNode.NODE_TYPE.LOW_OBSTACLE;
        }
        return NavNode.NODE_TYPE.NONE;
    }
}
