using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{
    public INTERACTION_TYPE m_interaction = INTERACTION_TYPE.NONE;

    public FACING_DIR m_facingDir = FACING_DIR.NONE;

    protected SquadManager squadManager;

    public Animator m_animator = null;
    public AgentAnimationController m_agentAnimationController = null;
    public AgentInventory m_agentInventory = null;

    [Header("DebugDebugging Only")]
    [Tooltip("Do Not Assign")]
    public bool m_turn = false;
    [Tooltip("Do Not Assign")]
    public bool m_knockedout = false;

    [Tooltip("# of actions unit can perform")]
    public int m_maxActionPoints = 2;
    public int m_currentActionPoints = 2;
    [Tooltip("Speed of an agent")]
    public float m_moveSpeed = 1.0f;

    [Space]
    public float m_highlightInteractablesRange = 6;

    protected TurnManager m_turnManager = null;
    public NavNode m_currentNavNode = null;

    public TurnManager.TEAM m_team = TurnManager.TEAM.AI;

    public Agent m_targetAgent = null;
    public Item m_targetItem = null;
    public Interactable m_targetInteractable = null;

    public Vector3 m_colliderExtents;

    [Header("Vision details")]
    public float m_visionFullOpacity = 1;
    public float m_visionFadeMaxOpacity = 0.5f;
    public float m_visionFadeMinOpacity = 0.3f;

    [Header("Full alertness")]
    [Tooltip("Distance in units, remember a tile is 2 metres's")]
    public int m_visionFullDistance = 5;
    [Tooltip("Total vision cone forwards, e.g. 60 is forwards, left/right 30 degrees")]
    public float m_visionFullAngle = 30;

    [Header("Investigation alertness")]
    [Tooltip("Distance in units, remember a tile is 2 metres's")]
    public int m_visionFadeDistance = 10;
    [Tooltip("Total vision cone forwards, e.g. 60 is forwards, left/right 30 degrees")]
    public float m_visionFadeAngle = 60;

    [SerializeField]
    public List<NavNode> m_path = new List<NavNode>();

    public int m_autoStandupTimer = 0;

    protected virtual void Start()
    {
        //New Stuff
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("NavNode")))
            m_currentNavNode = hit.collider.GetComponent<NavNode>();

        if (m_currentNavNode != null) // Current node isobstructed, that is taken up by agent
        {
            m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.OBSTRUCTED;
            m_currentNavNode.m_obstructingAgent = this;
        }

        m_animator = GetComponent<Animator>();
        m_agentAnimationController = GetComponent<AgentAnimationController>();
        m_agentInventory = GetComponent<AgentInventory>();

        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();

        m_colliderExtents = GetComponent<CapsuleCollider>().bounds.extents;

        m_facingDir = GetFacingDir(transform.forward);
    }

    //Start of turn, only runs once per turn
    public virtual void AgentTurnInit()
    {
        m_currentActionPoints = m_maxActionPoints;
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public virtual void AgentSelected() { }

    public enum AGENT_UPDATE_STATE {AWAITING_INPUT, PERFORMING_ACTIONS, END_TURN}
    //Constant update while agent is selected
    public virtual AGENT_UPDATE_STATE AgentTurnUpdate() { return AGENT_UPDATE_STATE.END_TURN; }

    //Runs when agent is removed from team list, end of turn
    public virtual void AgentTurnEnd(){}

	public void Knockout()
    {
		m_knockedout = true;

        //Fix node interactablility
        m_currentNavNode.m_obstructingAgent = null;
        m_currentNavNode.AddDownedAgent(this);
        m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.WALKABLE;

        //Setup animation
        m_agentAnimationController.m_animationSteps.Add(AnimationManager.ANIMATION_STEP.DEATH);
        m_agentAnimationController.PlayNextAnimation();

        m_turnManager.EndUnitTurn(this);

        m_autoStandupTimer = m_turnManager.m_autoStandupTime;
	}

    public void Revive()
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
        m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.WALKABLE;

        m_currentNavNode = navNode;

        m_currentNavNode.m_obstructingAgent = this;
        m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.OBSTRUCTED;
    }

    public static FACING_DIR GetFacingDir(Vector3 dir)
    {
        dir.y = 0; //Dont need to use y 
        float angle = Vector3.SignedAngle(dir.normalized, new Vector3(0, 0, 1), Vector3.up);

        if (angle < 10.0f && angle > -10.0f) //allows for minor inaccuracies
            return FACING_DIR.NORTH;
        if (angle > 170.0f || angle < -170.0f)
            return FACING_DIR.SOUTH;
        if (angle < 100 && angle > 70)
            return FACING_DIR.WEST;
        if (angle < -70 && angle > -100)
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
