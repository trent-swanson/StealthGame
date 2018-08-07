using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum ANIMATION_STEPS {IDLE = 0, RUN, CLIMB_UP, CLIMB_DOWN, WALL_IDLE, COUNT}

    public enum AT {IDLE_TO_RUN, RUN_TO_IDLE, IDLE_TO_CLIMB_UP, IDLE_TO_CLIMB_DOWN, RUN_TO_CLIMB_UP, RUN_TO_CLIMB_DOWN, NONE }//Animation transistions

    private static AT[,] m_stepsToAnimation = new AT[,]
    {// IDLE                    RUN                 CLIMB_UP                CLIMB_DOWN              WALL_IDLE 
        {AT.NONE,               AT.IDLE_TO_RUN,     AT.IDLE_TO_CLIMB_UP,    AT.IDLE_TO_CLIMB_DOWN,  AT.NONE},//IDLE to X
        {AT.RUN_TO_IDLE,        AT.NONE,            AT.RUN_TO_CLIMB_UP,     AT.RUN_TO_CLIMB_DOWN,   AT.NONE},//RUN to X
        {AT.NONE,               AT.RUN_TO_IDLE,     AT.IDLE_TO_CLIMB_UP,    AT.IDLE_TO_CLIMB_DOWN,  AT.NONE},//CLIMB_UP to X
        {AT.NONE,               AT.RUN_TO_IDLE,     AT.IDLE_TO_CLIMB_UP,    AT.IDLE_TO_CLIMB_DOWN,  AT.NONE},//CLIMB_DOWN to X
        {AT.NONE,               AT.NONE,            AT.NONE,                AT.NONE,                AT.NONE}//WALL_IDLE to X
    };

    public static List<AT> GetTransistionSteps(NavNode startingNode, List<NavNode> pathNodes)
    {
        List<AT> transitionSteps = new List<AT>();

        List<ANIMATION_STEPS> animationSteps = BuildAnimationList(startingNode, pathNodes);

        for (int i = 0; i < animationSteps.Count -1; i++)
        {
            transitionSteps.Add(m_stepsToAnimation[(int)animationSteps[i], (int)animationSteps[i + 1]]);
        }

        return transitionSteps;
    }

    private static List<ANIMATION_STEPS> BuildAnimationList(NavNode startingNode, List<NavNode> pathNodes)
    {
        List<ANIMATION_STEPS> pathSteps = new List<ANIMATION_STEPS>();

        pathSteps.Add(ANIMATION_STEPS.IDLE);//First node
        pathSteps.Add(GetMovementState(startingNode, pathNodes[0]));//First node

        for (int i = 0; i < pathNodes.Count-1; i++)
        {
            pathSteps.Add(GetMovementState(pathNodes[i], pathNodes[i + 1]));
        }

        if (pathSteps[pathSteps.Count - 1] == ANIMATION_STEPS.RUN)//On last instance, if running always set to idle TODO fix up
            pathSteps[pathSteps.Count - 1] = ANIMATION_STEPS.IDLE;

        return pathSteps;
    }

    private static ANIMATION_STEPS GetMovementState(NavNode currentNode, NavNode nextNode)
    {
        int nodeTileHeightDiff = nextNode.m_gridPos.y - currentNode.m_gridPos.y;

        Debug.Log(nodeTileHeightDiff);

        if (nodeTileHeightDiff == 0) //Stright path just run
            return ANIMATION_STEPS.RUN;
        else if (nodeTileHeightDiff > 0)//positive height diff, running up
        {
            return ANIMATION_STEPS.CLIMB_UP;
        }
        else if (nodeTileHeightDiff < 0)//negitive height diff, running down
        {
            return ANIMATION_STEPS.CLIMB_DOWN;
        }

        return ANIMATION_STEPS.IDLE;
    }

    //What triggers are needed to get transition to run
    public static void SetupAnimator(Animator animator, AT transition)
    {
        switch (transition)
        {
            case AT.IDLE_TO_RUN:
                animator.SetTrigger("Run");
                break;
            case AT.RUN_TO_IDLE:
                animator.SetTrigger("Idle");
                break;
            case AT.IDLE_TO_CLIMB_UP:
                animator.SetTrigger("ClimbUp");
                break;
            case AT.IDLE_TO_CLIMB_DOWN:
                animator.SetTrigger("ClimbDown");
                break;
            case AT.RUN_TO_CLIMB_UP:
                animator.SetTrigger("Idle");
                animator.SetTrigger("ClimbUp");
                break;
            case AT.RUN_TO_CLIMB_DOWN:
                animator.SetTrigger("Idle");
                animator.SetTrigger("ClimbDown");
                break;
            case AT.NONE:
                break;
            default:
                break;
        }
    }

}
