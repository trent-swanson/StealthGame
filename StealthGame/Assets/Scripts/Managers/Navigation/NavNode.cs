using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    [Header("Tile UI")]
    public GameObject m_selectableUI;
    public GameObject m_selectedUI;
    public GameObject m_NPCVisionUI;
    public Sprite m_selectedSprite;
    public Sprite m_attackSprite;
    public Sprite m_reviveSprite;
    public Sprite m_pickupSprite;
    public Sprite m_defaultSprite;
    public SpriteRenderer m_spriteRenderer;
    public SpriteRenderer m_requiredItem;

    public Item m_item = null;
    public Interactable m_interactable = null;

    //[0] = North, [1] = East, [2] = South, [3] = West
    public bool[] m_abilityToWallHide = new bool[4];

    [HideInInspector]
    public Color spriteColor;

    //auto assigned
    [Space]
    [Space]
    //BFS vars
    public NavNode m_BFSPreviousNode = null;
    public int m_BFSDistance = 0;

    public enum NODE_STATE { SELECTED, SELECTABLE, UNSELECTED }
    public enum NODE_TYPE { NONE, WALKABLE, INTERACTABLE, OBSTRUCTED, HIGH_OBSTACLE, LOW_OBSTACLE }

    public Agent m_obstructingAgent = null;
    public List<Agent> m_downedAgents = null;

    public NODE_STATE m_nodeState = NODE_STATE.UNSELECTED;
    public NODE_TYPE m_nodeType = NODE_TYPE.NONE;

    public Vector3 m_nodeTop = Vector3.zero;
    public Vector3Int m_gridPos = Vector3Int.zero;

    public List<NavNode> m_adjacentNodes = new List<NavNode>();

    //Division of navnodes into directions
    public List<NavNode> m_northNodes = new List<NavNode>();
    public List<NavNode> m_eastNodes = new List<NavNode>();
    public List<NavNode> m_southNodes = new List<NavNode>();
    public List<NavNode> m_westNodes = new List<NavNode>();

    public float m_gScore, m_hScore, m_fScore = 0;

    public NavNode m_previousNode = null;

    public List<Agent> m_tileVisibleToNPCs = new List<Agent>();

    void Start()
    {
        m_spriteRenderer = m_selectableUI.GetComponent<SpriteRenderer>();
        spriteColor = m_spriteRenderer.color;
        Vector3 colliderExtents = GetComponent<BoxCollider>().size;
        m_nodeTop = transform.position + Vector3.up * colliderExtents.y * transform.lossyScale.y;
    }

    public void Setup(List<NavNode> openNodes, List<NavNode> closedNodes, NavNode goalNode)
    {
        NavNode previousNode = null;
        float previousNodeCost = Mathf.Infinity;

        foreach (NavNode navNode in m_adjacentNodes)
        {
            if (navNode.m_gScore < previousNodeCost && (openNodes.Contains(navNode) || closedNodes.Contains(navNode)))
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

    public void SetNodeAsInteractable(Interactable interactable)
    {
        m_interactable = interactable;
        m_nodeType = NavNode.NODE_TYPE.INTERACTABLE;
    }

    public void SetupNodeType()
    {
        if (m_interactable != null)
        {
            m_nodeType = NavNode.NODE_TYPE.INTERACTABLE;
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, Navigation.m_obstacleDetection, LayerManager.m_enviromentLayer | LayerManager.m_navNodeLayer))
        {
            float coliderHeight = hit.collider.gameObject.GetComponent<BoxCollider>().size.y;
            if (coliderHeight < Navigation.m_lowObstacleHeight)
                m_nodeType = NavNode.NODE_TYPE.LOW_OBSTACLE;
            else
                m_nodeType = NavNode.NODE_TYPE.HIGH_OBSTACLE;
        }
        else
            m_nodeType = NavNode.NODE_TYPE.WALKABLE;
    }

    public void UpdateNavNodeState(NODE_STATE nodeState, Agent agent)
    {
        m_nodeState = nodeState;

        switch (nodeState) {
            case NODE_STATE.SELECTABLE:
                if (m_obstructingAgent != null && m_obstructingAgent.m_team != agent.m_team)
                {
                    m_selectableUI.SetActive(true);
                    m_selectedUI.SetActive(false);
                    m_spriteRenderer.sprite = m_attackSprite;
                }
                else if (m_interactable != null)
                {
                    m_selectableUI.SetActive(true);
                    m_selectedUI.SetActive(false);
                    m_spriteRenderer.sprite = m_pickupSprite;
                }
                else if (m_nodeType == NODE_TYPE.WALKABLE)
                {
                    if (GetDownedAgent(agent.m_team) != null)
                    {
                        m_selectableUI.SetActive(true);
                        m_selectedUI.SetActive(false);
                        m_spriteRenderer.sprite = m_reviveSprite;
                    }
                    else if (m_item != null)
                    {
                        m_selectableUI.SetActive(true);
                        m_selectedUI.SetActive(false);
                        m_spriteRenderer.sprite = m_pickupSprite;

                        //Show item highlight
                        if (m_item != null)
                            m_item.TurnOnOutline();
                    }
                    else
                    {
                        m_selectableUI.SetActive(true);
                        m_selectedUI.SetActive(false);
                        m_spriteRenderer.sprite = m_defaultSprite;
                    }
                }
                break;

            case NODE_STATE.SELECTED:
                if (m_obstructingAgent != null && m_obstructingAgent.m_team != agent.m_team)
                {
                    m_selectableUI.SetActive(true);
                    m_selectedUI.SetActive(false);
                    m_spriteRenderer.sprite = m_attackSprite;
                }
                else if (m_interactable != null)
                {
                    m_selectableUI.SetActive(true);
                    m_selectedUI.SetActive(true);
                    m_spriteRenderer.sprite = m_pickupSprite;
                }
                else if (m_nodeType == NODE_TYPE.WALKABLE)
                {
                    Agent downAgent = GetDownedAgent(agent.m_team);

                    if (downAgent != null)
                    {
                        m_selectableUI.SetActive(true);
                        m_selectedUI.SetActive(false);
                        m_spriteRenderer.sprite = m_reviveSprite;
                    }
                    else if (m_item != null)
                    {
                        m_selectableUI.SetActive(true);
                        m_selectedUI.SetActive(true);
                        m_spriteRenderer.sprite = m_pickupSprite;

                        //Show item highlight
                        if (m_item != null)
                            m_item.TurnOnOutline();
                    }
                    else
                    {
                        m_selectedUI.SetActive(true);
                        m_spriteRenderer.sprite = m_selectedSprite;

                    }
                }
                break;

            case NODE_STATE.UNSELECTED:
                m_selectableUI.SetActive(false);
                m_selectedUI.SetActive(false);
                m_spriteRenderer.sprite = m_defaultSprite;

                //Remove item highlight
                if (m_item != null)
                    m_item.TurnOffOutline();
                break;
        }
    }

    public void SetupWallHideIndicators(Navigation navigation)
    {
        m_abilityToWallHide[0] = AbilityToHide(this, m_northNodes);
        m_abilityToWallHide[2] = AbilityToHide(this, m_southNodes);
        m_abilityToWallHide[1] = AbilityToHide(this, m_eastNodes);
        m_abilityToWallHide[3] = AbilityToHide(this, m_westNodes);
    }

    private bool AbilityToHide(NavNode currentNode, List<NavNode> adjacentNodes)
    {
        foreach (NavNode adjacentNode in adjacentNodes)
        {
            int heightDiff = adjacentNode.m_gridPos.y - currentNode.m_gridPos.y;

            if (heightDiff == 0 && (adjacentNode.m_nodeType == NODE_TYPE.LOW_OBSTACLE || adjacentNode.m_nodeType == NODE_TYPE.HIGH_OBSTACLE))//Same height, check if obstructed
                return true;
            else if (heightDiff == 1)//Can hide against a tile one high
                return true;
            else if(heightDiff == -1 && adjacentNode.m_nodeType == NODE_TYPE.HIGH_OBSTACLE)// hidable when tile is one lower but its obstacle is at least two high
                return true;
        }
        return false;
    }

    public Agent GetDownedAgent(Agent.TEAM team)
    {
        foreach (Agent agent in m_downedAgents)
        {
            if (agent.m_team == team)
                return agent;
        }
        return null;
    }

    public void AddDownedAgent(Agent agent)
    {
        if (!m_downedAgents.Contains(agent))
            m_downedAgents.Add(agent);
    }

    public void RemoveDownedAgent(Agent agent)
    {
        m_downedAgents.Remove(agent);
    }

    public void NPCVision(ADD_REMOVE_FUNCTION functionType, Agent npc)
    {
        switch (functionType)
        {
            case ADD_REMOVE_FUNCTION.ADD:
                if (!m_tileVisibleToNPCs.Contains(npc))
                    m_tileVisibleToNPCs.Add(npc);
                break;
            case ADD_REMOVE_FUNCTION.REMOVE:
                m_tileVisibleToNPCs.Remove(npc);
                break;
            default:
                break;
        }

        if (m_tileVisibleToNPCs.Count > 0)
            m_NPCVisionUI.SetActive(true);
        else
            m_NPCVisionUI.SetActive(false);
    }

    public void NavNodeItem(ADD_REMOVE_FUNCTION functionType, Item item = null)
    {
        switch (functionType)
        {
            case ADD_REMOVE_FUNCTION.ADD:
                if (m_item == null)
                    m_item = item;
                break;
            case ADD_REMOVE_FUNCTION.REMOVE:
                m_item = null;
                break;
            default:
                break;
        }

        SetupNodeType();
    }

    public void NavNodeInteractable(ADD_REMOVE_FUNCTION functionType, Interactable interactable = null)
    {
        switch (functionType)
        {
            case ADD_REMOVE_FUNCTION.ADD:
                if (m_interactable == null)
                    m_interactable = interactable;
                break;
            case ADD_REMOVE_FUNCTION.REMOVE:
                m_interactable = null;
                break;
            default:
                break;
        }
    }

    public NavNode GetAdjacentNode(FACING_DIR facingDir)
    {
        int height = m_gridPos.y;
        List<NavNode> m_adjacentNodes = new List<NavNode>();
        switch (facingDir)
        {
            case FACING_DIR.NORTH:
                m_adjacentNodes = m_northNodes;
                break;
            case FACING_DIR.EAST:
                m_adjacentNodes = m_eastNodes;
                break;
            case FACING_DIR.SOUTH:
                m_adjacentNodes = m_southNodes;
                break;
            case FACING_DIR.WEST:
                m_adjacentNodes = m_westNodes;
                break;
            case FACING_DIR.NONE:
            default:
                return null;
        }

        foreach (NavNode adjacentNavNode in m_adjacentNodes)
        {
            if (height == adjacentNavNode.m_gridPos.y)
                return adjacentNavNode;
        }

        return null;
    }
}
