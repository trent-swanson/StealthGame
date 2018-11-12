using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{
    public enum TEAM { PLAYER, NPC };
    public TEAM m_team = TEAM.PLAYER;

    public INTERACTION_TYPE m_interaction = INTERACTION_TYPE.NONE;

    public FACING_DIR m_facingDir = FACING_DIR.NONE;

    public Animator m_characterAnimator = null;
    public AgentAnimationController m_agentAnimationController = null;

    public AgentInventory m_agentInventory = null;
    public InventoryUI m_agentInventoryUI = null;

    public CameraController m_cameraController = null;

    [Header("DebugDebugging Only")]
    [Tooltip("Do Not Assign")]
    public bool m_turn = false;
    [Tooltip("Do Not Assign")]
    public bool m_knockedout = false;

    public GameState_PlayerTurn m_playerTurn = null;
    public GameState_NPCTurn m_NPCTurn = null;

    public GameController m_gameController = null;

    [Tooltip("# of actions unit can perform")]
    public int m_maxActionPoints = 2;
    public int m_currentActionPoints = 2;
    [Tooltip("Speed of an agent")]
    public float m_moveSpeed = 1.0f;

    [Space]
    public float m_highlightInteractablesRange = 6;

    public NavNode m_currentNavNode = null;
    public NavNode m_lastFreeNavNode = null;

    public Agent m_targetAgent = null;
    public Item m_targetItem = null;
    public Interactable m_targetInteractable = null;

    public Vector3 m_colliderExtents;

    [Header("Vision details")]
    public float m_visionFullOpacity = 1;
    public float m_visionFadeMaxOpacity = 0.5f;

    [Header("Full alertness")]
    [Tooltip("Distance in units, remember a tile is 2 metres's")]
    public int m_visionFullDistance = 5;
    [Tooltip("Total vision cone forwards, e.g. 60 is forwards, left/right 30 degrees")]
    public float m_visionFullAngle = 30;

    [HideInInspector]
    public List<NavNode> m_path = new List<NavNode>();

    protected virtual void Start()
    {
#if UNITY_EDITOR
        if (m_characterAnimator == null)
            Debug.Log("character animator needs to be assigned");
#endif
        //New Stuff
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("NavNode")))
            m_currentNavNode = hit.collider.GetComponent<NavNode>();

        if (m_currentNavNode != null) // Current node isobstructed, that is taken up by agent
        {
            m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.OBSTRUCTED;
            m_currentNavNode.m_obstructingAgent = this;
        }

        m_agentAnimationController = GetComponent<AgentAnimationController>();
        m_agentInventory = GetComponent<AgentInventory>();

        m_playerTurn = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState_PlayerTurn>();
        m_NPCTurn = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState_NPCTurn>();
        m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        m_cameraController = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();

        m_colliderExtents = GetComponent<CapsuleCollider>().bounds.extents;

        m_facingDir = GetFacingDir(transform.forward);
    }

    //Start of turn, only runs once per turn
    public virtual void AgentTurnInit()
    {
        m_currentActionPoints = m_maxActionPoints;
        m_lastFreeNavNode = m_currentNavNode;
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public virtual void AgentSelected() { }

    public enum AGENT_UPDATE_STATE {AWAITING_INPUT, PERFORMING_ACTIONS, END_TURN}
    //Constant update while agent is selected
    public virtual AGENT_UPDATE_STATE AgentTurnUpdate() { return AGENT_UPDATE_STATE.END_TURN; }

    //Runs when agent is removed from team list, end of turn
    public virtual void AgentTurnEnd(){}

	public virtual void Knockout()
    {
		m_knockedout = true;

        //Fix node interactablility
        m_currentNavNode.m_obstructingAgent = null;
        m_currentNavNode.AddDownedAgent(this);
        m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.WALKABLE;
        m_currentActionPoints = 0;

        //Setup animation
        m_agentAnimationController.m_animationSteps.Clear();
        m_agentAnimationController.m_animationSteps.Add(AnimationManager.ANIMATION_STEP.DEATH);
        m_agentAnimationController.PlayNextAnimation();
    }

    public virtual void Revive()
    {
        m_knockedout = false;

        //Fix node interactablility
        m_currentNavNode.m_obstructingAgent = this;
        m_currentNavNode.RemoveDownedAgent(this);
        m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.OBSTRUCTED;

        //Setup animation
        m_agentAnimationController.m_animationSteps.Add(AnimationManager.ANIMATION_STEP.REVIVE);
        m_agentAnimationController.PlayNextAnimation();
    }

    public void ChangeCurrentNavNode(NavNode navNode)
    {
        m_currentNavNode.m_obstructingAgent = null;
        m_currentNavNode.SetupNodeType();

        m_currentNavNode = navNode;

        if (m_currentNavNode.m_obstructingAgent == null) //Remeber last free navNode
            m_lastFreeNavNode = m_currentNavNode;

        m_currentNavNode.m_obstructingAgent = this;
        m_currentNavNode.SetupNodeType();
    }

    public static FACING_DIR GetFacingDir(Vector3 dir)
    {
        dir.y = 0; //Dont need to use y 
        float angle = Vector3.SignedAngle(dir.normalized, new Vector3(0, 0, 1), Vector3.up);

        if (angle < 45.0f && angle > -45.0f) //allows for minor inaccuracies
            return FACING_DIR.NORTH;
        if (angle > 135.0f || angle < -135.0f)
            return FACING_DIR.SOUTH;
        if (angle < 135 && angle > 45)
            return FACING_DIR.WEST;
        if (angle < -45 && angle > -135)
            return FACING_DIR.EAST;
        return FACING_DIR.NONE;
    }

    public static Vector3 FacingDirEnumToVector3(FACING_DIR facingDir)
    {
        switch (facingDir)
        {
            case FACING_DIR.NORTH:
                return Vector3.forward;
            case FACING_DIR.EAST:
                return Vector3.right;
            case FACING_DIR.SOUTH:
                return -Vector3.forward;
            case FACING_DIR.WEST:
                return -Vector3.right;
            case FACING_DIR.NONE:
            default:
                return Vector3.zero;
        }
    }
}
