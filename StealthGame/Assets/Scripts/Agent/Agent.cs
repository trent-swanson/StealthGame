﻿using System.Collections;
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
    protected SoundController m_soundController = null;

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

    //--------------------------------------------------------------------------------------
    // Initialisation
    //--------------------------------------------------------------------------------------
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

        m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        m_playerTurn = m_gameController.GetComponent<GameState_PlayerTurn>();
        m_soundController = m_gameController.GetComponent<SoundController>();
        m_NPCTurn = m_gameController.GetComponent<GameState_NPCTurn>();

        m_cameraController = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();

        m_colliderExtents = GetComponent<CapsuleCollider>().bounds.extents;

        m_facingDir = GetFacingDir(transform.forward);
    }

    //--------------------------------------------------------------------------------------
    // Start of turn, only runs once per turn
    //--------------------------------------------------------------------------------------
    public virtual void AgentTurnInit()
    {
        m_currentActionPoints = m_maxActionPoints;
    }

    //--------------------------------------------------------------------------------------
    //Runs every time a agent is selected, this can be at end of an action is completed
    //--------------------------------------------------------------------------------------
    public virtual void AgentSelected() { }

    public enum AGENT_UPDATE_STATE {AWAITING_INPUT, PERFORMING_ACTIONS, END_TURN}
    //Constant update while agent is selected
    public virtual AGENT_UPDATE_STATE AgentTurnUpdate() { return AGENT_UPDATE_STATE.END_TURN; }

    //--------------------------------------------------------------------------------------
    //Runs when agent is removed from team list, end of turn
    //-------------------------------------------------------------------------------------- 
    public virtual void AgentTurnEnd(){}

    //--------------------------------------------------------------------------------------
    //Agent gets knocked out, setup tile, play death animation
    //-------------------------------------------------------------------------------------- 
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

    //--------------------------------------------------------------------------------------
    //Agent gets revived,play revive animation. No AP added
    //-------------------------------------------------------------------------------------- 
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

    //--------------------------------------------------------------------------------------
    //Change the current state of agent tile/ Update new tile
    //-------------------------------------------------------------------------------------- 
    public void ChangeCurrentNavNode(NavNode navNode)
    {
        m_currentNavNode.m_obstructingAgent = null;
        m_currentNavNode.SetupNodeType();

        m_currentNavNode = navNode;

        m_currentNavNode.m_obstructingAgent = this;
        m_currentNavNode.SetupNodeType();
    }

    //--------------------------------------------------------------------------------------
    // Convert Vector3 direction to closest facing direction
    // 
    // Param
    //		dir: direction to be converted, doesnt have to be normalised
    // Return:
    //      closest facing direction, defualt to none
    //--------------------------------------------------------------------------------------
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

    //--------------------------------------------------------------------------------------
    // Convert facing direction to vector3
    // 
    // Param
    //		facingDir: direection to convert
    // Return:
    //      direction based off facing dir, defualt to Vector.zero
    //--------------------------------------------------------------------------------------
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

    //Animation Sounds
    private int m_walkSoundIndex = 0;
    public void PlayWalk()
    {
        switch (m_walkSoundIndex)
        {
            case 0:
                m_soundController.PlaySound(SoundController.SOUND.WALK_0);
                m_walkSoundIndex++;
                break;
            case 1:
                m_soundController.PlaySound(SoundController.SOUND.WALK_1);
                m_walkSoundIndex++;
                break;
            case 2:
                m_soundController.PlaySound(SoundController.SOUND.WALK_2);
                m_walkSoundIndex++;
                break;
            case 3:
                m_soundController.PlaySound(SoundController.SOUND.WALK_3);
                m_walkSoundIndex++;
                break;
            case 4:
                m_soundController.PlaySound(SoundController.SOUND.WALK_4);
                m_walkSoundIndex = 0;
                break;
            default:
                break;
        }
    }

    public void PlaySlideStart()
    {
        m_soundController.PlaySound(SoundController.SOUND.SLIDE_2);
    }

    public void PlaySlideEnd()
    {
        m_soundController.PlaySound(SoundController.SOUND.SLIDE_3);
    }

    public void PlayAttack()
    {
        m_soundController.PlaySound(SoundController.SOUND.PUNCH);
    }

    public void PlayDeath()
    {
        m_soundController.PlaySound(SoundController.SOUND.DEATH);
    }

    //--------------------------------------------------------------------------------------
    // Initialisation
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Convert Vector3 direction to closest facing direction
    // 
    // Param
    //		dir: direction to be converted, doesnt have to be normalised
    // Return:
    //      closest facing direction, defualt to none
    //--------------------------------------------------------------------------------------
}
