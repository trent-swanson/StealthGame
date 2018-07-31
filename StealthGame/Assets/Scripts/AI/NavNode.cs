using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    [Header("Tile UI")]
    public GameObject selectableUI;
    public GameObject selectedUI;
    public Sprite selectedSprite;
    public Sprite defualtSprite;
    public SpriteRenderer spriteRenderer;

    [HideInInspector]
    public Color spriteColor;

    
    //auto assigned
    [Space]
    [Space]

    //BFS vars
    public bool visited = false;
    public NavNode parent = null;
    public int distance = 0;
    public Agent selectableBy;

    Renderer myRenderer;

    public enum NodeState {CURRENT, SELECTED, SELECTABLE, UNSELECTED}
    public enum NodeType {WALKABLE, HIGH_OBSTICLE, LOW_OBSTICLE}

    public NodeState nodeState = NodeState.UNSELECTED;
    public NodeType nodeType;

    public Vector3Int m_gridPos = Vector3Int.zero;

    public List<NavNode> m_adjacentNodes = new List<NavNode>();

    public float m_gScore, m_hScore, m_fScore = 0;

    public NavNode m_previousNode = null;

    void Start() {
        myRenderer = GetComponent<Renderer>();
        spriteRenderer = selectableUI.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
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

    public void UpdateNavNodeState(NodeState p_nodeState) {
        nodeState = p_nodeState;
        switch (nodeState) {
            case NodeState.CURRENT:
                selectableUI.SetActive(true);
            break;
            case NodeState.SELECTABLE:
                selectableUI.SetActive(true);
                selectedUI.SetActive(false);
                spriteRenderer.sprite = defualtSprite;
            break;
            case NodeState.SELECTED:
                selectedUI.SetActive(true);
                spriteRenderer.sprite = selectedSprite;
            break;
            case NodeState.UNSELECTED:
            default:
                selectableUI.SetActive(false);
                selectedUI.SetActive(false);
                spriteRenderer.sprite = defualtSprite;
            break;
        }              
    }

    public void Reset() {
        nodeState = NodeState.UNSELECTED;
    }
}
