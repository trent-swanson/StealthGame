using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : ScriptableObject
{
    //SingletonSet
    private static Navigation m_instance = null;
    private Navigation() { }

    public static Navigation Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = CreateInstance<Navigation>();
                m_instance.Init();
            }
            return m_instance;
        }
    }

    private int m_maxLevelSize = 50;
    private int m_minYPos = -100;

    [Tooltip("How big a tile is in world units")]
    public Vector2 m_tileSize = new Vector2(2.0f, 2.0f);

    private int m_navNodeLayer = 0;

    //Caching of offsets for efficeincy
    private static Vector3 m_forwardOffset;
    private static Vector3 m_backwardOffset;
    private static Vector3 m_rightOffset;
    private static Vector3 m_leftOffset;

    //Storage of grid extents
    private int m_maxX = 0;
    private int m_minX = 0;
    private int m_maxZ = 0;
    private int m_minZ = 0;


    public NavNode[,] m_navGrid;
    public int m_navGridWidth;
    public int m_navGridHeight;

    public void Init()
    {
        //Setup stuff
        m_navNodeLayer = LayerMask.GetMask("NavNode");

        m_forwardOffset = new Vector3(0, 0, m_tileSize.y);
        m_backwardOffset = new Vector3(0, 0, -m_tileSize.y);
        m_rightOffset = new Vector3(m_tileSize.y, 0, 0);
        m_leftOffset = new Vector3(-m_tileSize.y, 0, 0);

        //TODO see if this is a good idea
        Vector3 orginatingPos = new Vector3(0.0f, m_minYPos, 0.0f);
        orginatingPos.y = m_minYPos;
        NavNode[,] tempNavNodeGrid = new NavNode[m_maxLevelSize, m_maxLevelSize];
        Vector2Int gridCenter = new Vector2Int(m_maxLevelSize / 2, m_maxLevelSize / 2);

        m_maxX = m_minX = gridCenter.x;
        m_maxZ = m_minZ = gridCenter.y;

        if (GetNavNode(orginatingPos, gridCenter, tempNavNodeGrid))
            UpdateNeighbourNodes(gridCenter, tempNavNodeGrid);

        //Create new grid clipping excess null filled sides
        m_navGridWidth = m_maxX - m_minX + 1;
        m_navGridHeight = m_maxZ - m_minZ + 1;
        m_navGrid = new NavNode[m_navGridWidth, m_navGridHeight];

        for (int i = 0; i < m_navGridWidth; i++)
        {
            for (int j = 0; j < m_navGridHeight; j++)
            {
                m_navGrid[i, j] = tempNavNodeGrid[m_minX + i, m_minZ + j];
                if(m_navGrid[i, j]!= null)
                    m_navGrid[i, j].m_gridPos = new Vector2Int(i, j);
            }
        }

        //Setup Node neighbours
        for (int i = 0; i < m_navGridWidth; i++)
        {
            for (int j = 0; j < m_navGridHeight; j++)
            {
                BuildNodeBranches(new Vector2Int(i, j));
            }
        }
    }

    //----------------
    //Creation of Nav Grid
    //----------------
    private void UpdateNeighbourNodes(Vector2Int currentGridPos, NavNode[,] navNodeGrid)
    {
        //Given a node, get the four surrounding nodes
        NavNode currentNode = navNodeGrid[currentGridPos.x, currentGridPos.y];

        Vector3 initialRaycastPos = currentNode.transform.position;
        initialRaycastPos.y = m_minYPos;

        //Forward
        if (GetNavNode(initialRaycastPos + m_forwardOffset, currentNode.m_gridPos + Vector2Int.up, navNodeGrid))
            UpdateNeighbourNodes(currentNode.m_gridPos + Vector2Int.up, navNodeGrid);

        //Backward
        if (GetNavNode(initialRaycastPos + m_backwardOffset, currentNode.m_gridPos + Vector2Int.down, navNodeGrid))
            UpdateNeighbourNodes(currentNode.m_gridPos + Vector2Int.down, navNodeGrid);

        //Right
        if (GetNavNode(initialRaycastPos + m_rightOffset, currentNode.m_gridPos + Vector2Int.right, navNodeGrid))
            UpdateNeighbourNodes(currentNode.m_gridPos + Vector2Int.right, navNodeGrid);

        //Left
        if (GetNavNode(initialRaycastPos + m_leftOffset, currentNode.m_gridPos + Vector2Int.left, navNodeGrid))
            UpdateNeighbourNodes(currentNode.m_gridPos + Vector2Int.left, navNodeGrid);

        navNodeGrid[currentGridPos.x, currentGridPos.y] = currentNode;
    }

    private bool GetNavNode(Vector3 pos, Vector2Int gridPos, NavNode[,] navNodeGrid)
    {
        if(navNodeGrid[gridPos.x, gridPos.y] != null)//Case of a node already exisitng
            return false;

        RaycastHit hit;
        if(Physics.Raycast(pos, Vector3.up, out hit, Mathf.Infinity, m_navNodeLayer))
        {
            NavNode navNode = hit.collider.GetComponent<NavNode>();

            if (navNode != null)
            {
                navNode.m_gridPos = gridPos;
                navNodeGrid[gridPos.x, gridPos.y] = navNode;

                m_minX = gridPos.x < m_minX ? gridPos.x : m_minX;
                m_maxX = gridPos.x > m_maxX ? gridPos.x : m_maxX;
                m_minZ = gridPos.y < m_minZ ? gridPos.y : m_minZ;
                m_maxZ = gridPos.y > m_maxZ ? gridPos.y : m_maxZ;

                return true;
            }
        }
        return false;
    }

    private void BuildNodeBranches(Vector2Int currentGridPos)
    {
        NavNode currentNode = m_navGrid[currentGridPos.x, currentGridPos.y];

        if(currentNode!=null)
        {
            //Forward
            if (currentGridPos.y + 1 < m_navGridHeight && m_navGrid[currentGridPos.x, currentGridPos.y + 1] != null)
                currentNode.m_adjacentNodes.Add(m_navGrid[currentGridPos.x, currentGridPos.y + 1]);
            //Backward
            if (currentGridPos.y - 1 >= 0 && m_navGrid[currentGridPos.x, currentGridPos.y - 1] != null)
                currentNode.m_adjacentNodes.Add(m_navGrid[currentGridPos.x, currentGridPos.y - 1]);
            //Right
            if (currentGridPos.x + 1 < m_navGridWidth && m_navGrid[currentGridPos.x + 1, currentGridPos.y] != null)
                currentNode.m_adjacentNodes.Add(m_navGrid[currentGridPos.x + 1, currentGridPos.y]);
            //Left
            if (currentGridPos.x - 1 >= 0 && m_navGrid[currentGridPos.x - 1, currentGridPos.y] != null)
                currentNode.m_adjacentNodes.Add(m_navGrid[currentGridPos.x - 1, currentGridPos.y]);
        }

        m_navGrid[currentGridPos.x, currentGridPos.y] = currentNode;
    }
    //----------------
    //End of Nav Grid Creation
    //----------------

    //----------------
    //A* stuff
    //----------------
    public List<NavNode> GetNavPath(NavNode startingNode, NavNode goalNode)
    {
        if (startingNode == goalNode)//Already at position
            return null;

        List<NavNode> openNodes = new List<NavNode>();
        List<NavNode> closedNodes = new List<NavNode>();

        //Get starting node
        openNodes.Add(startingNode);

        NavNode currentNode = startingNode;

        //Loop till no more options
        while (openNodes.Count > 0)
        {
            //Break early when at end
            if (currentNode == goalNode)
                return GetPath(currentNode, startingNode);

            AddNextNodes(currentNode, goalNode, openNodes, closedNodes);

            currentNode = GetLowestFScore(openNodes);
        }
        return null;
    }

    public List<NavNode> GetNavPath(Vector2Int startingIndexPos, NavNode goalNode)
    {
        NavNode startingNode = m_navGrid[startingIndexPos.x, startingIndexPos.y];
        return GetNavPath(startingNode, goalNode);
    }

    private void AddNextNodes(NavNode currentNode, NavNode goalNode, List<NavNode> openNodes, List<NavNode> closedNodes)
    {
        openNodes.Remove(currentNode);
        closedNodes.Add(currentNode);

        foreach (NavNode nextNode in currentNode.m_adjacentNodes)
        {
            if (!openNodes.Contains(nextNode) && !closedNodes.Contains(nextNode))
            {
                openNodes.Add(nextNode);
                nextNode.Setup(openNodes, closedNodes, goalNode);
            }
        }
    }

    private NavNode GetLowestFScore(List<NavNode> openNodes)
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

    private List<NavNode> GetPath(NavNode currentNode, NavNode startingNode)
    {
        List<NavNode> path = new List<NavNode>();
        while(currentNode != startingNode)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.m_previousNode;
        }
        path.Add(currentNode);
        return path;
    }
}
