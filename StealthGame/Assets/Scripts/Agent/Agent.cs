using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Agent : MonoBehaviour {

    protected SquadManager squadManager;

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

    [Space]
    public List<Item> m_currentItems = new List<Item>();


    public Animator m_animator = null;
    public Animation m_rotateAnimation;
    private static float m_rotateAnimationTime;

    protected virtual void Start()
    {
        //New Stuff
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("NavNode")))
            m_currentNavNode = hit.collider.GetComponent<NavNode>();

        if (m_currentNavNode != null) // Current node isobstructed, that is taken up by player
            m_currentNavNode.m_nodeState = NavNode.NODE_STATE.OBSTRUCTED;

        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();

        m_animator = GetComponentInChildren<Animator>();


        //https://answers.unity.com/questions/692593/get-animation-clip-length-using-animator.html 
        //Getting animation clip length from animator
        AnimationClip[] animatorClips= m_animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < animatorClips.Length; i++)   
        {
            if (animatorClips[i].name == "idleToTurnLeftToIdleNoRoot")   
            {
                m_rotateAnimationTime = animatorClips[i].length * 0.8f;//reduction on turning to allow for minor float inacuracies 
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Rotate(Agent.ROTATION_DIR.LEFT);
    }

    //Start of turn, only runs once per turn
    public virtual void AgentTurnInit()
    {
        m_currentActionPoints = m_maxActionPoints;
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public virtual void AgentSelected() { }

    //Constant update while agent is selected
    public virtual void AgentTurnUpdate() { }

    //Runs when agent is removed from team list, end of turn
    public virtual void AgentTurnEnd(){}

	public void Knockout()
    {
		m_knockedout = true;
		transform.position = new Vector3(0, 100, 0);
        m_turnManager.EndUnitTurn(this);
	}

    public virtual bool IsMoving() { return false; }


    public enum ROTATION_DIR {LEFT = -1, RIGHT = 1 }

    public IEnumerator Rotate(ROTATION_DIR rotationDir)
    {
        float totalRotateAmount = 90 * (int)rotationDir;

        int steps = (int)(m_rotateAnimationTime / Time.fixedDeltaTime);
        float steptime = m_rotateAnimationTime / steps;
        float stepAmount = totalRotateAmount / steps;

        Quaternion finalRotation = transform.rotation * Quaternion.Euler(0, 90 * (int)rotationDir, 0);

        for (int i = 0; i < steps; i++)
        {
            StartCoroutine(RotateOverTime(i * steptime, stepAmount));
        }

        yield return new WaitForSeconds(m_rotateAnimationTime);
        transform.rotation = finalRotation;
    }

    public IEnumerator RotateOverTime(float delay, float rotateAmount)
    {
        yield return new WaitForSeconds(delay);
        transform.RotateAround(transform.position, Vector3.up, rotateAmount);
    }
}
