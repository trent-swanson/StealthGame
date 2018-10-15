using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    private LineRenderer m_pathRenderer = null;
    private UIController m_uiController = null;
    private PlayerController m_playerController = null;
    private AgentInventory m_agentInventory;

    [Space]
    [Space]
    [Header("Unit Editable Variables")]
    public GameObject m_unitCanvas;
    public Image m_APDisplay;
    public Sprite m_defualtAPDisplay;
    public Sprite m_selectedAPDisplay;
    public TextMeshProUGUI m_APNumber;

    private List<Item> m_visiblePickups = new List<Item>();

    public enum MESH_STATE { DRAW_NAVMESH, DRAW_PATH, REMOVE_NAVMESH, REMOVE_PATH}

    private void Start()
    {
        m_pathRenderer = GetComponentInChildren<LineRenderer>();
        m_uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        m_playerController = GetComponent<PlayerController>();
        m_agentInventory = GetComponent<AgentInventory>();

        m_unitCanvas.SetActive(true);
        m_APDisplay.rectTransform.sizeDelta = new Vector2(-0.14f, -0.16f);
    }

    public void InitUI()
    {
        m_APDisplay.sprite = m_defualtAPDisplay;
        m_APDisplay.rectTransform.sizeDelta = new Vector2(-0.14f, -0.16f);
        UpdateAPDisplay(m_playerController.m_currentActionPoints);
    }

    public void StartUI()
    {
        m_APDisplay.sprite = m_selectedAPDisplay;
        m_APDisplay.rectTransform.sizeDelta = new Vector2(0, 0);

        ShowInteractables();
    }

    public void EndUI()
    {
        m_APDisplay.sprite = m_defualtAPDisplay;
        m_APDisplay.rectTransform.sizeDelta = new Vector2(-0.14f, -0.16f);

        RemoveInteractables();
    }

    public void UpdateUI()
    {
        if(m_playerController!=null)
        {
            ShowInteractables();
            m_uiController.UpdateItemInventory(m_playerController);
            UpdateAPDisplay(m_playerController.m_currentNavNode.m_BFSDistance);
        }
    }

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

            //Update AP
            if (newSelectedNode != null)
                UpdateAPDisplay(newSelectedNode.m_BFSDistance);
        }
        else if (state == MESH_STATE.REMOVE_PATH) //remove path to selected node
        {
            if (currentSelectedNode != null)
                currentSelectedNode.UpdateNavNodeState(NavNode.NODE_STATE.SELECTABLE, m_playerController);

            ClearPathRender();

            //Update AP
            UpdateAPDisplay(m_playerController.m_currentActionPoints);
        }
        else if (state == MESH_STATE.REMOVE_NAVMESH) //remove all visualisation
        {
            SetNodeStates(selectableNodes, NavNode.NODE_STATE.UNSELECTED);
            ClearPathRender();
        }
    }

    private void SetNodeStates(List<NavNode> navNodes, NavNode.NODE_STATE state)
    {
        foreach (NavNode navNode in navNodes)
        {
            navNode.UpdateNavNodeState(state, m_playerController);

            SpriteRenderer spriteRenderer = navNode.m_selectableUI.GetComponent<SpriteRenderer>();
            Color newColor = spriteRenderer.color;

            if (navNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED)//Full view for obstructed tiles
                newColor.a = 1;
            else
                newColor.a = (float)(navNode.m_BFSDistance + 1) / m_playerController.m_currentActionPoints;//min val of 1/current action points
            spriteRenderer.color = newColor;
        }
    }

    private void CalculatePathRender(List<NavNode> nodePath)
    {
        Vector3[] pathPos = GetPathPos(nodePath);
        m_pathRenderer.positionCount = pathPos.Length;
        m_pathRenderer.SetPositions(pathPos);
    }

    private void ClearPathRender()
    {
        m_pathRenderer.positionCount = 0;
    }

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

    public void UpdateAPDisplay(int APLeft)
    {
        m_APNumber.text = APLeft.ToString();
    }

    //highlight interactable objects in range
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
    }

    private void RemoveInteractables()
    {
        foreach (Item pickUp in m_visiblePickups)
        {
            pickUp.TurnOffOutline();
        }
    }
}
