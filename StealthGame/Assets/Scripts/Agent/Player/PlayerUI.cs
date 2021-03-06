﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour
{
    private LineRenderer m_pathRenderer = null;
    private UIController m_uiController = null;
    private PlayerController m_playerController = null;
    private AgentInventory m_agentInventory;

    private List<Item> m_visiblePickups = new List<Item>();

    public Image m_APImage = null;
    public Sprite m_selectedSprite = null;
    public Sprite m_unselectedSprite = null;
    public enum MESH_STATE { DRAW_NAVMESH, DRAW_PATH, REMOVE_NAVMESH, REMOVE_PATH}

    //--------------------------------------------------------------------------------------
    // Initialisation
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        m_pathRenderer = GetComponentInChildren<LineRenderer>();
        m_uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        m_playerController = GetComponent<PlayerController>();
        m_agentInventory = GetComponent<AgentInventory>();

#if UNITY_EDITOR
        if (m_APImage == null)
            Debug.Log("Player UI need to have the AP canvas applied");
#endif
    }

    //--------------------------------------------------------------------------------------
    // On players turn start, display UI
    //--------------------------------------------------------------------------------------
    public void StartUI()
    {
        ShowInteractables();
        m_playerController.m_agentInventoryUI.UpdateInventory(m_agentInventory);
        m_APImage.sprite = m_selectedSprite;
    }

    //--------------------------------------------------------------------------------------
    // On players turn end, remove UI
    //--------------------------------------------------------------------------------------
    public void EndUI()
    {
        RemoveInteractables();
        m_APImage.sprite = m_unselectedSprite;
    }

    //--------------------------------------------------------------------------------------
    // During players turn update UI as needed
    //--------------------------------------------------------------------------------------
    public void UpdateUI()
    {
        if(m_playerController!=null)
        {
            ShowInteractables();
            m_uiController.UpdateItemInventory(m_playerController);
        }
    }

    //--------------------------------------------------------------------------------------
    // Given a navnode setup its UI
    // 
    // Param
    //		state: state of nav node UI
    //		selectableNodes: List of all selectable navNodes
    //		currentSelectedNode: Whatis currently selcted navnode, has different UI colour
    //		newSelectedNode: What is the new slected navnode, on change from previous selected, swap UI
    //		nodePath: path to draw using path renderer
    //--------------------------------------------------------------------------------------
    public void UpdateNodeVisualisation(MESH_STATE state, List<NavNode> selectableNodes, NavNode currentSelectedNode = null, NavNode newSelectedNode = null, List<NavNode> nodePath = null)
    {
        if (state == MESH_STATE.DRAW_NAVMESH) //draw just navmesh
        {
            SetNodeStates(selectableNodes, NavNode.NODE_STATE.SELECTABLE);
        }
        else if (state == MESH_STATE.DRAW_PATH) //draw path to selected node TODO tidy up
        {
            if (currentSelectedNode != null)
                currentSelectedNode.UpdateNavNodeState(NavNode.NODE_STATE.SELECTABLE, m_playerController);

            if (newSelectedNode != null)
                newSelectedNode.UpdateNavNodeState(NavNode.NODE_STATE.SELECTED, m_playerController);

            //Redraw path
            ClearPathRender();
            CalculatePathRender(nodePath);
        }
        else if (state == MESH_STATE.REMOVE_PATH) //remove path to selected node
        {
            SetNodeStates(selectableNodes, NavNode.NODE_STATE.SELECTABLE);

            ClearPathRender();
        }
        else if (state == MESH_STATE.REMOVE_NAVMESH) //remove all visualisation
        {
            SetNodeStates(selectableNodes, NavNode.NODE_STATE.UNSELECTED);
            ClearPathRender();
        }
    }

    //--------------------------------------------------------------------------------------
    // Set up Nav node UI, alter sprite renderer/transparancy 
    // 
    // Param
    //		navNodes: all navnodes to alter UI of
    //		state: state of nav node UI
    //--------------------------------------------------------------------------------------
    private void SetNodeStates(List<NavNode> navNodes, NavNode.NODE_STATE state)
    {
        foreach (NavNode navNode in navNodes)
        {
            navNode.UpdateNavNodeState(state, m_playerController);

            SpriteRenderer spriteRenderer = navNode.m_selectableUI.GetComponent<SpriteRenderer>();
            Color newColor = spriteRenderer.color;

            if (navNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || navNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE || navNode.m_item != null || navNode.m_downedAgents.Count > 0)//Full view for obstructed tiles
                newColor.a = 1;
            else
                newColor.a = (float)(navNode.m_BFSDistance + 1) / m_playerController.m_currentActionPoints;//min val of 1/current action points
            spriteRenderer.color = newColor;
        }
    }

    //--------------------------------------------------------------------------------------
    // Set up path renderer
    // 
    // Param
    //		nodePath: path of navnodes
    //--------------------------------------------------------------------------------------
    private void CalculatePathRender(List<NavNode> nodePath)
    {
        Vector3[] pathPos = GetPathPos(nodePath);
        m_pathRenderer.positionCount = pathPos.Length;
        m_pathRenderer.SetPositions(pathPos);
    }

    //--------------------------------------------------------------------------------------
    // Remove path renderer
    //--------------------------------------------------------------------------------------
    private void ClearPathRender()
    {
        if(m_pathRenderer!=null)
            m_pathRenderer.positionCount = 0;
    }

    //--------------------------------------------------------------------------------------
    // Get path positions
    // Postion starts from top of nav node plus some
    // On navnode change in height, add aditional positions on edges
    // 
    // Param
    //		nodes: NavNodes to build path from
    //--------------------------------------------------------------------------------------
    private Vector3[] GetPathPos(List<NavNode> nodes)
    {
        List<Vector3> pathPos = new List<Vector3>();
        for (int i = 0; i < nodes.Count; i++)
        {
            pathPos.Add(nodes[i].m_nodeTop + Vector3.up * 0.1f);//Center node

            //Add to edge
            if(i+1 < nodes.Count)
            {
                if(nodes[i].m_gridPos.y < nodes[i + 1].m_gridPos.y) // Moving up
                {
                    Vector3 dir = nodes[i + 1].transform.position - nodes[i].transform.position;
                    dir.y = 0;
                    pathPos.Add(nodes[i].m_nodeTop + (Vector3.up * 0.1f) + (dir * 0.45f));//Box to edge
                    pathPos.Add(nodes[i + 1].m_nodeTop + (Vector3.up * 0.1f) - (dir * 0.55f));//Edge to center of next box
                }
                else if(nodes[i].m_gridPos.y > nodes[i + 1].m_gridPos.y)// Moving dwn node
                {
                    Vector3 dir = nodes[i + 1].transform.position - nodes[i].transform.position;
                    dir.y = 0;
                    pathPos.Add(nodes[i].m_nodeTop + (Vector3.up * 0.1f) + (dir * 0.55f));//Box to edge
                    pathPos.Add(nodes[i + 1].m_nodeTop + (Vector3.up * 0.1f) - (dir * 0.45f));//Edge to center of next box
                }
            }
        }
        return pathPos.ToArray();
    }
    
    //--------------------------------------------------------------------------------------
    // Highlight interactable objects in range
    //--------------------------------------------------------------------------------------
    private void ShowInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_playerController.m_highlightInteractablesRange, LayerManager.m_interactableLayer);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Item pickup = hitColliders[i].GetComponent<Item>(); //TODO change pickup to interactable
            if (pickup!=null)
            {
                m_visiblePickups.Add(pickup);
                pickup.TurnOnOutline();
            }
        }

        m_uiController.ShowInteractables(m_agentInventory);
    }

    //--------------------------------------------------------------------------------------
    // Remove highlighted interactable objects
    //--------------------------------------------------------------------------------------
    private void RemoveInteractables()
    {
        foreach (Item pickUp in m_visiblePickups)
        {
            pickUp.TurnOffOutline();
        }
    }
}
