using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementManager : MonoBehaviour
{
    private TurnManager m_turnManager = null;
    private int m_navNodeLayer = 0;

    public NavNode m_currentSelectedNode = null;
    public PlayerController m_currentPlayer = null;
    public LineRenderer m_pathRenderer = null;

    private List<NavNode> m_selectableNodes = new List<NavNode>();

    public enum MESH_STATE { DRAW, REMOVE}
    public MESH_STATE m_currentDrawState = MESH_STATE.REMOVE;

    private void Start()
    {
        m_currentPlayer = GetComponent<PlayerController>();
        m_pathRenderer = GetComponentInChildren<LineRenderer>();
        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
        m_navNodeLayer = LayerMask.GetMask("NavNode");
    }

    public void MovementManage()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_navNodeLayer))
        {
            NavNode navNode = hit.collider.GetComponent<NavNode>();

            if (navNode != null && m_selectableNodes.Contains(navNode))//Over node hats selectable
            {
                UpdateVisualisation(MESH_STATE.DRAW, navNode);

                if (Input.GetMouseButtonUp(0))//Case Hover
                {

                }
                else //Case mouse down
                {

                }
            }
            else //Node is not selectable
            {
                UpdateVisualisation(MESH_STATE.REMOVE, null);
            }
        }
        else // Hovering over nothing
        {
            UpdateVisualisation(MESH_STATE.REMOVE, null);
        }
    }

    private void CreateSelectableArea()
    {
        //Remove old selectable nodes
        SetNodeStates(m_selectableNodes, NavNode.NODE_STATE.UNSELECTED);
        m_selectableNodes.Clear();
        GetAllSelectableNodes();

        SetNodeStates(m_selectableNodes, NavNode.NODE_STATE.SELECTABLE);
    }

    private void SetNodeStates(List<NavNode> navNodes, NavNode.NODE_STATE state)
    {
        foreach (NavNode navNode in navNodes)
        {
            navNode.UpdateNavNodeState(state);
            if(navNode.m_nodeState == NavNode.NODE_STATE.SELECTABLE)
            {
                SpriteRenderer spriteRenderer = navNode.m_selectableUI.GetComponent<SpriteRenderer>();
                Color newColor = spriteRenderer.color;
                newColor.a = 1 - navNode.m_BFSDistance * 0.3f;
                spriteRenderer.color = newColor;
            }
        }
    }

    private void GetAllSelectableNodes()
    {
        m_selectableNodes.Clear();

        int distance = m_currentPlayer.m_currentActionPoints;

        NavNode currentNavNode = m_currentPlayer.m_currentNavNode;

        m_selectableNodes.Add(currentNavNode);

        currentNavNode.m_BFSDistance = distance;
        currentNavNode.m_BFSPreviousNode = null;

        UpdateNeighbourBFS(currentNavNode);
    }

    private void UpdateNeighbourBFS(NavNode currentNode)
    {
        //Get neighbours
        foreach (NavNode nextNavNode in currentNode.m_adjacentNodes)
        {
            int newDistance = currentNode.m_BFSDistance - 1; //TODO addiitoinal cost when moving up or down; //Wow adding semicolons to comments, Ive been doing this for far too long...

            if (m_selectableNodes.Contains(nextNavNode))
            {
                if(newDistance > nextNavNode.m_BFSDistance)
                {
                    nextNavNode.m_BFSDistance = newDistance;
                    nextNavNode.m_BFSPreviousNode = currentNode;

                    UpdateNeighbourBFS(nextNavNode);
                }
            }
            else if(newDistance >= 0)
            {
                m_selectableNodes.Add(nextNavNode);

                nextNavNode.m_BFSDistance = newDistance;
                nextNavNode.m_BFSPreviousNode = currentNode;

                UpdateNeighbourBFS(nextNavNode);
            }
        }
    }

    private List<NavNode> GetPath(NavNode endNode)
    {
        List<NavNode> path = new List<NavNode>();

        NavNode currentNode = endNode;
        while (currentNode!=null)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_BFSPreviousNode;
        }

        return path;
    }

    private Vector3[] GetPathPos(List<NavNode> nodes)
    {
        Vector3[] pathPos = new Vector3[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            pathPos[i] = nodes[i].transform.position + Vector3.up * 0.6f;
        }
        return pathPos;
    }

    private void CalculatePathRender(Vector3[] path)
    {
        m_pathRenderer.positionCount = path.Length;
        m_pathRenderer.SetPositions(path);
    }

    private void ClearPathRender()
    {
        m_pathRenderer.positionCount = 0;
    }

    public void PlayerSelected()
    {
        GetAllSelectableNodes();
    }

    public void UpdateVisualisation(MESH_STATE state, NavNode newSelected)
    {
        if(state == MESH_STATE.DRAW && m_currentDrawState == MESH_STATE.REMOVE) // removed -> showing selction
        {
            m_currentDrawState = state;

            CalculatePathRender(GetPathPos(GetPath(newSelected)));
            SetNodeStates(m_selectableNodes, NavNode.NODE_STATE.SELECTABLE);

            if(m_currentSelectedNode != null)
                m_currentSelectedNode.UpdateNavNodeState(NavNode.NODE_STATE.SELECTED);
        }
        else if(state == MESH_STATE.DRAW && newSelected != m_currentSelectedNode) // New selected node
        {
            m_currentDrawState = state;

            if (m_currentSelectedNode != null)
                m_currentSelectedNode.UpdateNavNodeState(NavNode.NODE_STATE.SELECTABLE);

            //Redraw path
            ClearPathRender();
            CalculatePathRender(GetPathPos(GetPath(newSelected)));

            newSelected.UpdateNavNodeState(NavNode.NODE_STATE.SELECTED);

            m_currentSelectedNode = newSelected;
        }
        else if(m_currentDrawState != MESH_STATE.REMOVE && state == MESH_STATE.REMOVE) //remove visualisation
        {
            m_currentDrawState = state;

            if (m_currentSelectedNode != null)
                m_currentSelectedNode.UpdateNavNodeState(NavNode.NODE_STATE.SELECTABLE);

            ClearPathRender();
        }
    }

}
