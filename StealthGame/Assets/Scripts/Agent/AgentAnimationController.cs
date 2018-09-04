using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimationController : MonoBehaviour
{
    private Agent m_agent = null;
    private Animator m_animator = null;
    private Animation m_rotateAnimation;
    public string m_rotateLeftRightName = "TurnRight";
    public  float m_rotateLeftRightTime;
    public string m_rotateTurnAroundName = "TurnAround";
    public float m_rotateTurnAroundTime;
    [Tooltip("The number of 'frames' to rotate")]
    public static int m_rotationSteps = 16;

    public bool m_playNextAnimation = true;
    public string m_currentAnimation = "Idle";

    public List<AnimationManager.ANIMATION_STEP> m_animationSteps = new List<AnimationManager.ANIMATION_STEP>();

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
                m_rotateLeftRightTime = animatorClips[i].length * 0.7f;//reduction on turning to allow for minor float inacuracies '
            }
            else if(animatorClips[i].name == m_rotateTurnAroundName)
            {
                m_rotateTurnAroundTime = animatorClips[i].length * 0.7f;//reduction on turning to allow for minor float inacuracies '
            }
        }
    }

    public void EndAnimationState()
    {
        m_animator.SetBool(m_currentAnimation, false);
        m_playNextAnimation = true;
    }

    public void PlayNextAnimation()
    {
        if (m_animationSteps.Count > 0)
        {
            switch (m_animationSteps[0])
            {
                case AnimationManager.ANIMATION_STEP.IDLE:
                    m_currentAnimation = "Idle";
                    break;
                case AnimationManager.ANIMATION_STEP.STEP:
                    m_currentAnimation = "Step";
                    break;
                case AnimationManager.ANIMATION_STEP.WALK:
                    m_currentAnimation = "Walk";
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_UP_IDLE:
                    m_currentAnimation = "JumpToIdle";
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_UP_WALK:
                    m_currentAnimation = "JumpToWalk";
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_DOWN_IDLE:
                    m_currentAnimation = "DropToIdle";
                    break;
                case AnimationManager.ANIMATION_STEP.CLIMB_DOWN_WALK:
                    m_currentAnimation = "DropToWalk";
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
                    break;
                case AnimationManager.ANIMATION_STEP.TURN_LEFT:
                    m_currentAnimation = "TurnLeft";
                    RotateLeftRight(ROTATION_DIR.LEFT);
                    break;
                case AnimationManager.ANIMATION_STEP.TURN_AROUND:
                    m_currentAnimation = "TurnAround";
                    RotateTurnAround();
                    break;
                case AnimationManager.ANIMATION_STEP.INTERACTION:
                    m_currentAnimation = "Interact";
                    break;
                case AnimationManager.ANIMATION_STEP.ATTACK:
                    if (m_agent.m_attackingTarget != null)
                        m_agent.m_attackingTarget.Knockout();
                    m_currentAnimation = "Attack";
                    break;
                case AnimationManager.ANIMATION_STEP.DEATH:
                    m_currentAnimation = "Death";
                    break;
                default:
                    break;
            }
            m_animator.SetBool(m_currentAnimation, true);
            m_playNextAnimation = false;
        }
    }

    public enum ROTATION_DIR { LEFT = -1, RIGHT = 1}

    public void RotateLeftRight(ROTATION_DIR rotationDir)
    {
        float totalRotateAmount = 90 * (int)rotationDir;
        StartCoroutine(Rotate(totalRotateAmount, m_rotateLeftRightTime));
    }

    public void RotateTurnAround()
    {
        float totalRotateAmount = 180;
        StartCoroutine(Rotate(totalRotateAmount, m_rotateTurnAroundTime));
    }

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

    public IEnumerator RotateOverTime(float delay, float rotateAmount)
    {
        yield return new WaitForSeconds(delay);
        transform.RotateAround(transform.position, Vector3.up, rotateAmount);
    }
}
