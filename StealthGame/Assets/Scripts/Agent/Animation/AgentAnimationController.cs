using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimationController : MonoBehaviour
{
    private Agent m_agent = null;
    private Animator m_animator = null;
    public string m_rotateLeftRightName = "TurnRight";
    public float m_rotateLeftRightTime;
    public string m_rotateTurnAroundName = "TurnAround";
    public float m_rotateTurnAroundTime;
    [Tooltip("The number of 'frames' to rotate")]
    public static int m_rotationSteps = 16;

    public bool m_playNextAnimation = true;
    public string m_currentAnimation = "Idle";

    public List<AnimationManager.ANIMATION_STEP> m_animationSteps = new List<AnimationManager.ANIMATION_STEP>();

    private GameState_NPCTurn m_NPCTurn = null;
    private GameState_PlayerTurn m_playerTurn = null;

    //--------------------------------------------------------------------------------------
    // Initialisation
    // Prep turning time
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        m_agent = GetComponent<Agent>();
        m_animator = GetComponentInChildren<Animator>();

        //https://answers.unity.com/questions/692593/get-animation-clip-length-using-animator.html 
        //Getting animation clip length from animator
        AnimationClip[] animatorClips = m_animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < animatorClips.Length; i++)
        {
            if (animatorClips[i].name == m_rotateLeftRightName)
            {
                m_rotateLeftRightTime = animatorClips[i].length * 0.9f;//reduction on turning to allow for minor float inacuracies '
            }
            else if (animatorClips[i].name == m_rotateTurnAroundName)
            {
                m_rotateTurnAroundTime = animatorClips[i].length * 0.9f;//reduction on turning to allow for minor float inacuracies '
            }
        }

        m_NPCTurn = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState_NPCTurn>();
        m_playerTurn = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState_PlayerTurn>();
    }

    //--------------------------------------------------------------------------------------
    // Called by animtion event, lets controller know when animtation has ended
    //--------------------------------------------------------------------------------------
    public void EndAnimationState()
    {
        if (m_animationSteps.Count > 0)
        {
            m_animationSteps.RemoveAt(0);
        }

        m_animator.SetBool(m_currentAnimation, false);
        m_playNextAnimation = true;

        PlayNextAnimation();
    }

    //--------------------------------------------------------------------------------------
    // Play the next set animtion based off m_animationSteps
    //--------------------------------------------------------------------------------------
    public void PlayNextAnimation()
    {
        if (m_animationSteps.Count > 0)
        {
            switch (m_animationSteps[0])
            {
                case AnimationManager.ANIMATION_STEP.IDLE:
                    m_currentAnimation = "Idle";
                    UpdateGridPos();//Has to check on idle, as some moves end in idle e.g. run to idle => idle
                    break;
                case AnimationManager.ANIMATION_STEP.STEP:
                    m_currentAnimation = "Step";
                    UpdateGridPos();
                    break;
                case AnimationManager.ANIMATION_STEP.WALK:
                    m_currentAnimation = "Walk";
                    UpdateGridPos();
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_UP_IDLE:
                    m_currentAnimation = "JumpToIdle";
                    UpdateGridPos();
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_UP_WALK:
                    m_currentAnimation = "JumpToRun";
                    UpdateGridPos();
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_DOWN_IDLE:
                    m_currentAnimation = "DropToIdle";
                    UpdateGridPos();
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_DOWN_WALK:
                    m_currentAnimation = "DropToRun";
                    UpdateGridPos();
                    break;
                case AnimationManager.ANIMATION_STEP.WALL_HIDE_RIGHT:
                    m_currentAnimation = "WallRight";
                    break;
                case AnimationManager.ANIMATION_STEP.WALL_HIDE_LEFT:
                    m_currentAnimation = "WallLeft";
                    break;
                case AnimationManager.ANIMATION_STEP.TURN_RIGHT:
                    m_currentAnimation = "TurnRight";
                    RotateLeftRight(ROTATION_DIR.RIGHT);
                    RotateFacingDir(m_agent, ROTATION_DIR.RIGHT);
                    break;
                case AnimationManager.ANIMATION_STEP.TURN_LEFT:
                    m_currentAnimation = "TurnLeft";
                    RotateLeftRight(ROTATION_DIR.LEFT);
                    RotateFacingDir(m_agent, ROTATION_DIR.LEFT);
                    break;
                case AnimationManager.ANIMATION_STEP.TURN_AROUND:
                    m_currentAnimation = "TurnAround";
                    RotateTurnAround();
                    RotateFacingDir(m_agent, ROTATION_DIR.RIGHT);
                    RotateFacingDir(m_agent, ROTATION_DIR.RIGHT);
                    break;
                case AnimationManager.ANIMATION_STEP.INTERACTABLE:
                    Interactable interactable = m_agent.m_targetInteractable;
                    if (interactable != null && interactable.CanPerform(m_agent))
                    {
                        interactable.PerformAction(m_agent);

                        if (interactable.m_customAnimation == "")
                            m_currentAnimation = "Interact";
                        else
                            m_currentAnimation = interactable.m_customAnimation;
                    }
                    else
                        m_currentAnimation = "";
                    break;
                case AnimationManager.ANIMATION_STEP.PICKUP_ITEM:
                    Item item = m_agent.m_targetItem;
                    if (item != null)
                        item.EquipItem(m_agent);
                    m_currentAnimation = "Pickup";
                    break;
                case AnimationManager.ANIMATION_STEP.ATTACK:
                    //Determine atrtack direction, if guard knows of player then guard attacks, else same as normal
                    PlayerController playerAttacker = m_agent.GetComponent<PlayerController>();

                    if (playerAttacker != null)
                    {
                        NPC NPCDefender = playerAttacker.m_targetAgent.GetComponent<NPC>();

                        if (NPCDefender != null && NPCDefender.KnowsOfPlayer(playerAttacker))
                        {
                            //Add rotation animaiton
                            AnimationManager.GetRotation(ref NPCDefender.m_facingDir, Agent.GetFacingDir(playerAttacker.m_currentNavNode.m_nodeTop - NPCDefender.m_currentNavNode.m_nodeTop), ref NPCDefender.m_agentAnimationController.m_animationSteps);

                            //Add attacking
                            NPCDefender.m_agentAnimationController.m_animationSteps.Add(AnimationManager.ANIMATION_STEP.ATTACK);
                            NPCDefender.m_agentAnimationController.PlayNextAnimation();

                            NPCDefender.UpdateWorldState();
                            NPCDefender.RemoveTarget(playerAttacker);

                            playerAttacker.Knockout();
                        }
                        else
                        {
                            NPCDefender.Knockout();
                            m_currentAnimation = "Attack";
                        }
                    }
                    else
                    {
                        if (m_agent.m_targetAgent != null)
                            m_agent.m_targetAgent.Knockout();
                        m_currentAnimation = "Attack";
                    }
                    break;
                case AnimationManager.ANIMATION_STEP.RANGED_ATTACK:
                    if (m_agent.m_targetAgent != null)
                        m_agent.m_targetAgent.Knockout();
                    m_currentAnimation = "RangedAttack";
                    break;
                case AnimationManager.ANIMATION_STEP.WALL_ATTACK:
                    if (m_agent.m_targetAgent != null)
                        m_agent.m_targetAgent.Knockout();

                    FACING_DIR attackingDir = Agent.GetFacingDir(m_agent.m_targetAgent.transform.position - m_agent.transform.position);

                    int dirAmount = (int)attackingDir - (int)m_agent.m_facingDir;

                    switch (dirAmount)
                    {
                        case 1:
                        case -3:
                            m_currentAnimation = "WallAttackRight";
                            break;
                        case -1:
                        case 3:
                            m_currentAnimation = "WallAttackLeft";
                            break;
                        case 0:
                        case 2:
                            m_currentAnimation = "WallAttackForward";
                            break;
                    }

                    PlayerController playerController = m_agent.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.m_playerStateMachine.m_currentlyHiding = true;
                    }
                    break;
                case AnimationManager.ANIMATION_STEP.DEATH:
                    m_currentAnimation = "Death";
                    break;
                case AnimationManager.ANIMATION_STEP.REVIVE:
                    if (m_agent.m_targetAgent != null)
                        m_agent.m_targetAgent.Revive();
                    m_currentAnimation = "Revive";
                    break;
                default:
                    break;
            }

            if (m_currentAnimation != "")
            {
                m_animator.SetBool(m_currentAnimation, true);
                m_playNextAnimation = false;

                m_NPCTurn.UpdateNPCWorldStates();
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // On a movemnt based animation, update node based off agent path
    //--------------------------------------------------------------------------------------
    public void UpdateGridPos()
    {
        List<NavNode> path = m_agent.m_path;
        if (path.Count > 1)
        {
            path.RemoveAt(0);

            m_agent.ChangeCurrentNavNode(path[0]);

            m_agent.m_path = path;
        }
    }

    public enum ROTATION_DIR { LEFT = -1, RIGHT = 1 }

    //--------------------------------------------------------------------------------------
    // On a rotation based animation, update facing direction
    // Start rotation coroutein
    // 
    // Param
    //		rotationDir: direction to rotate towards
    //--------------------------------------------------------------------------------------
    public void RotateLeftRight(ROTATION_DIR rotationDir)
    {
        float totalRotateAmount = 90 * (int)rotationDir;
        StartCoroutine(Rotate(totalRotateAmount, m_rotateLeftRightTime));
    }

    //--------------------------------------------------------------------------------------
    // On a rotation based animation, update facing direction
    // Always a 180 turn
    // Start rotation coroutein
    //--------------------------------------------------------------------------------------
    public void RotateTurnAround()
    {
        float totalRotateAmount = 180;
        StartCoroutine(Rotate(totalRotateAmount, m_rotateTurnAroundTime));
    }

    //--------------------------------------------------------------------------------------
    // Rotation of agent over turning time
    // setup all rotation corouteins
    // 
    // Param
    //		angle: angle to roate in each step
    //		totalTime: how long the rotation will take
    //--------------------------------------------------------------------------------------
    public IEnumerator Rotate(float angle, float totalTime)
    {
        float steptime = totalTime / m_rotationSteps;
        float stepAmount = angle / m_rotationSteps;

        Quaternion finalRotation = transform.rotation * Quaternion.Euler(0, angle, 0);

        for (int i = 0; i < m_rotationSteps; i++)
        {
            StartCoroutine(RotateOverTime(i * steptime, stepAmount));
        }

        yield return new WaitForSeconds(totalTime);
        transform.rotation = finalRotation;
    }

    //--------------------------------------------------------------------------------------
    // Rotation of agent in fixed step
    // 
    // Param
    //		delay:  long to wait till roation occurs
    //		rotateAmount: how far to rotate
    //--------------------------------------------------------------------------------------
    public IEnumerator RotateOverTime(float delay, float rotateAmount)
    {
        yield return new WaitForSeconds(delay);
        transform.RotateAround(transform.position, Vector3.up, rotateAmount);
    }

    //--------------------------------------------------------------------------------------
    // Given forwards dir, roate left or right. E.g. facing north, turn left, facing now west
    // 
    // Param
    //		agent: agent to base foward values off
    //		rotationDir: left/right
    //--------------------------------------------------------------------------------------
    public void RotateFacingDir(Agent agent, ROTATION_DIR rotationDir)
    {
        FACING_DIR currentDir = agent.m_facingDir;
        int newDir = (int)currentDir + (int)rotationDir;

        if (newDir == -1)
            newDir = 3;
        else if (newDir == 4)
            newDir = 0;

        agent.m_facingDir = (FACING_DIR)newDir;
    }
}
