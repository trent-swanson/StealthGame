using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    [Header("Tile UI")]
    public GameObject m_selectableUI;
    public GameObject m_selectedUI;
    public Sprite m_selectedSprite;
    public Sprite m_defaultSprite;
    public SpriteRenderer m_spriteRenderer;

    [HideInInspector]
    public Color spriteColor;

    
    //auto assigned
    [Space]
    [Space]

    //BFS vars
    public NavNode m_BFSPreviousNode = null;
    public int m_BFSDistance = 0;

    Renderer myRenderer;

    public enum NODE_STATE {CURRENT, SELECTED, SELECTABLE, UNSELECTED}
    public enum NODE_TYPE {WALKABLE, HIGH_OBSTACLE, LOW_OBSTACLE}

    public NODE_STATE m_nodeState = NODE_STATE.UNSELECTED;
    public NODE_TYPE m_nodeType = NODE_TYPE.WALKABLE;

    public Vector3Int m_gridPos = Vector3Int.zero;

    public List<NavNode> m_adjacentNodes = new List<NavNode>();

    public float m_gScore, m_hScore, m_fScore = 0;

    public NavNode m_previousNode = null;

    void Start() {
        myRenderer = GetComponent<Renderer>();
        m_spriteRenderer = m_selectableUI.GetComponent<SpriteRenderer>();
        spriteColor = m_spriteRenderer.color;
    }

    public NavNode(Vector3Int pos)
    {
        m_gridPos = pos;
    }

    public void Setup(List<NavNode> openNodes, List<NavNode> closedNodes, NavNode goalNode)
    {
        NavNode previousNode = null;
        float previousNodeCost = Mathf.Infinity;

        foreach (NavNode navNode in m_adjacentNodes)
        {
            if(navNode.m_gScore < previousNodeCost && (openNodes.Contains(navNode) || closedNodes.Contains(navNode)))
            {
                previousNode = navNode;
                previousNodeCost = navNode.m_gScore;
            }
        }

        m_previousNode = previousNode;
        m_gScore = previousNodeCost + 1;

        m_hScore = Vector3.Distance(transform.position, goalNode.transform.position);

        m_fScore = m_hScore + m_gScore;
    }

    public bool IsOccupied()
    {
        return false;
    }

    public void UpdateNavNodeState(NODE_STATE nodeState) {
        m_nodeState = nodeState;
        switch (nodeState) {
            case NODE_STATE.CURRENT:
                m_selectableUI.SetActive(true);
            break;
            case NODE_STATE.SELECTABLE:
                m_selectableUI.SetActive(true);
                m_selectedUI.SetActive(false);
                m_spriteRenderer.sprite = m_defaultSprite;
            break;
            case NODE_STATE.SELECTED:
                m_selectedUI.SetActive(true);
                m_spriteRenderer.sprite = m_selectedSprite;
            break;

            case NODE_STATE.UNSELECTED:
            default:
                m_selectableUI.SetActive(false);
                m_selectedUI.SetActive(false);
                m_spriteRenderer.sprite = m_defaultSprite;
            break;
        }              
    }
}
