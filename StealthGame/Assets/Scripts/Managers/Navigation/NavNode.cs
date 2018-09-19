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

    public static float m_wallHideSelectionDeadZone = 0.8f;

    public Item m_item = null;
    public Interactable m_interactable = null;

    [System.Serializable]
    public struct WallHideIndicators
    {
        public SpriteRenderer m_wallHideSprite;
        public SpriteRenderer m_wallHideGroundSprite;
        public NODE_TYPE m_wallHideType;
        public bool m_selected;
    }

    [SerializeField]
    public WallHideIndicators[] m_wallHideIndicators = new WallHideIndicators[4];

    [HideInInspector]
    public Color spriteColor;

    //auto assigned
    [Space]
    [Space]
    //BFS vars
    public NavNode m_BFSPreviousNode = null;
    public int m_BFSDistance = 0;

    Renderer myRenderer;

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
        myRenderer = GetComponent<Renderer>();
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

    public void SetupNodeType()
    {
        if(m_interactable!=null)
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

                    ToggleWallHideIndicators(false);
                }
                else if (m_nodeType == NODE_TYPE.INTERACTABLE)
                {
                    m_selectableUI.SetActive(true);
                    m_selectedUI.SetActive(false);
                    m_spriteRenderer.sprite = m_pickupSprite;

                    ToggleWallHideIndicators(false);
                }
                    break;

            case NODE_STATE.SELECTED:
                if (m_obstructingAgent != null && m_obstructingAgent.m_team != agent.m_team)
                {
                    m_selectableUI.SetActive(true);
                    m_selectedUI.SetActive(false);
                    m_spriteRenderer.sprite = m_attackSprite;
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
                    else if(m_item!= null)
                    {
                        m_selectableUI.SetActive(true);
                        m_selectedUI.SetActive(true);
                        m_spriteRenderer.sprite = m_pickupSprite;

                        //Show item highlight
                        if (m_item != null)
                            m_item.TurnOnOutline();

                        ToggleWallHideIndicators(true);
                    }
                    else
                    {
                        m_selectedUI.SetActive(true);
                        m_spriteRenderer.sprite = m_selectedSprite;

                        ToggleWallHideIndicators(true);
                    }
                }
                else if (m_nodeType == NODE_TYPE.INTERACTABLE)
                {
                    m_selectableUI.SetActive(true);
                    m_selectedUI.SetActive(true);
                    m_spriteRenderer.sprite = m_pickupSprite;

                    ToggleWallHideIndicators(false);
                }
                break;

            case NODE_STATE.UNSELECTED:
                m_selectableUI.SetActive(false);
                m_selectedUI.SetActive(false);
                m_spriteRenderer.sprite = m_defaultSprite;

                //Remove item highlight
                if(m_item!=null)
                    m_item.TurnOffOutline();
                break;
        }
    }

    public void SetupWallHideIndicators(Navigation navigation)
    {
        m_wallHideIndicators[0].m_wallHideType = navigation.GetAdjacentNodeType(m_gridPos, new Vector3Int(0, 0, 1));
        m_wallHideIndicators[1].m_wallHideType = navigation.GetAdjacentNodeType(m_gridPos, new Vector3Int(1, 0, 0));
        m_wallHideIndicators[2].m_wallHideType = navigation.GetAdjacentNodeType(m_gridPos, new Vector3Int(0, 0, -1));
        m_wallHideIndicators[3].m_wallHideType = navigation.GetAdjacentNodeType(m_gridPos, new Vector3Int(-1, 0, 0));
    }

    public void UpdateWallIndicators()
    {
        if (m_nodeType == NODE_TYPE.OBSTRUCTED && m_obstructingAgent.m_team == TurnManager.TEAM.AI)//No need to update wall hide indicators on nodes with enemy on them
            return;

        Color halfAlpha = new Color(1, 1, 1, 0.2f);
        for (int i = 0; i < 4; i++)
        {
            m_wallHideIndicators[i].m_wallHideSprite.color = halfAlpha;
            m_wallHideIndicators[i].m_wallHideGroundSprite.color = halfAlpha;
            m_wallHideIndicators[0].m_selected = false;
        }

        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        RaycastHit hit;

        //TODO make better
        m_wallHideIndicators[0].m_selected = false;
        m_wallHideIndicators[1].m_selected = false;
        m_wallHideIndicators[2].m_selected = false;
        m_wallHideIndicators[3].m_selected = false;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))
        {
            Vector3 relativeMousePos = hit.point - transform.position;

            if (Mathf.Abs(relativeMousePos.z) > Mathf.Abs(relativeMousePos.x))//North south Indicator
            {
                if (relativeMousePos.z > m_wallHideSelectionDeadZone)//North Indicator
                {
                    if (m_wallHideIndicators[0].m_wallHideType == NODE_TYPE.LOW_OBSTACLE || m_wallHideIndicators[0].m_wallHideType == NODE_TYPE.HIGH_OBSTACLE)
                    {
                        m_wallHideIndicators[0].m_wallHideSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[0].m_wallHideGroundSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[0].m_selected = true;
                    }
                }
                else if (relativeMousePos.z < -m_wallHideSelectionDeadZone) //South indicator
                {
                    if (m_wallHideIndicators[2].m_wallHideType == NODE_TYPE.LOW_OBSTACLE || m_wallHideIndicators[2].m_wallHideType == NODE_TYPE.HIGH_OBSTACLE)
                    {
                        m_wallHideIndicators[2].m_wallHideSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[2].m_wallHideGroundSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[2].m_selected = true;
                    }
                }
            }
            else
            {
                if (relativeMousePos.x > m_wallHideSelectionDeadZone)//East Indicator
                {
                    if (m_wallHideIndicators[1].m_wallHideType == NODE_TYPE.LOW_OBSTACLE || m_wallHideIndicators[1].m_wallHideType == NODE_TYPE.HIGH_OBSTACLE)
                    {
                        m_wallHideIndicators[1].m_wallHideSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[1].m_wallHideGroundSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[1].m_selected = true;
                    }
                }
                else if (relativeMousePos.x < -m_wallHideSelectionDeadZone) //West indicator
                {
                    if (m_wallHideIndicators[3].m_wallHideType == NODE_TYPE.LOW_OBSTACLE || m_wallHideIndicators[3].m_wallHideType == NODE_TYPE.HIGH_OBSTACLE)
                    {
                        m_wallHideIndicators[3].m_wallHideSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[3].m_wallHideGroundSprite.color = new Color(1, 1, 1, 1);
                        m_wallHideIndicators[3].m_selected = true;
                    }
                }
            }
        }
    }

    public void ToggleWallHideIndicators(bool toggleVal)
    {
        for (int i = 0; i < 4; i++) //Add wall hide icons
        {
            m_wallHideIndicators[i].m_selected = false;

            if (m_wallHideIndicators[i].m_wallHideType == NODE_TYPE.LOW_OBSTACLE || m_wallHideIndicators[i].m_wallHideType == NODE_TYPE.HIGH_OBSTACLE)
            {
                m_wallHideIndicators[i].m_wallHideSprite.enabled = toggleVal;
                m_wallHideIndicators[i].m_wallHideGroundSprite.enabled = toggleVal;
            }
            else
            {
                m_wallHideIndicators[i].m_wallHideSprite.enabled = false;
                m_wallHideIndicators[i].m_wallHideGroundSprite.enabled = false;
            }
        }
    }

    public FACING_DIR GetWallHideDir()
    {
        for (int i = 0; i < 4; i++)
        {
            if (m_wallHideIndicators[i].m_selected == true)
            {
                return (FACING_DIR)i;//Casting 'i' to direction
            }
        }
        return FACING_DIR.NONE;
    }

    public Agent GetDownedAgent(TurnManager.TEAM team)
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
}
