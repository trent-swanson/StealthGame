using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum ANIMATION_STEP {IDLE, STEP, RUN, CLIMB_UP_IDLE, CLIMB_UP_RUN, CLIMB_DOWN_IDLE, CLIMB_DOWN_RUN, WALL_HIDE_RIGHT, WALL_HIDE_LEFT, ATTACK, RANGED_ATTACK, INTERACTION }//Animation states

    public static List<ANIMATION_STEP> GetAnimationSteps(List<NavNode> pathNodes, Interactable interactable = null)
    {
        List<ANIMATION_STEP> transitionSteps = new List<ANIMATION_STEP>();

        int pathCount = pathNodes.Count;

        if (pathCount < 2)
            return transitionSteps;

        if (pathCount == 2)
        {
            ANIMATION_STEP step = GetActionSteps(pathNodes[0], pathNodes[1]);
            if(step == ANIMATION_STEP.RUN)
                transitionSteps.Add(ANIMATION_STEP.STEP);//On a single step on same level want to "walk"
            else
                transitionSteps.Add(step);
            return transitionSteps;
        }

        for (int i = 0; i < pathCount - 2; i++)
        {
            transitionSteps.Add(GetActionSteps(pathNodes[i], pathNodes[i + 1], pathNodes[i + 2]));
        }

        if(interactable != null)//Adding action to end, e.g. pick up weapon
        {
            if(interactable.m_requiresIdle)
            {
                transitionSteps.Add(ANIMATION_STEP.IDLE);
                transitionSteps.Add(ANIMATION_STEP.INTERACTION);
            }
            else
            {
                if(interactable.m_interactableType == Interactable.INTERACTABLE_TYPE.WALL_HIDE)
                    transitionSteps.Add(ANIMATION_STEP.WALL_HIDE_RIGHT);
                else
                    transitionSteps.Add(ANIMATION_STEP.INTERACTION);
            }
        }
        else //Always default to idle
        {
            ANIMATION_STEP lastStep = GetActionSteps(pathNodes[pathCount - 2], pathNodes[pathCount - 1]);

            if (lastStep == ANIMATION_STEP.RUN)
                transitionSteps.Add(ANIMATION_STEP.IDLE);
            else
                transitionSteps.Add(lastStep);
        }

        return transitionSteps;
    }

    private static ANIMATION_STEP GetActionSteps(NavNode currentNode, NavNode nextNode, NavNode futureNode = null)
    {
        int nodeHeightDiff = nextNode.m_gridPos.y - currentNode.m_gridPos.y;

        if (nodeHeightDiff == 0) //Stright path just run
            return ANIMATION_STEP.RUN;
        else if (nodeHeightDiff > 0)//positive height diff, running up
        {
            if(futureNode==null)
                return ANIMATION_STEP.CLIMB_UP_IDLE;
            else
            {
                int futureNodeHeightDiff = futureNode.m_gridPos.y - nextNode.m_gridPos.y;
                if (nodeHeightDiff == 0) //Stright path just run
                    return ANIMATION_STEP.CLIMB_UP_RUN;
                else //All other case momentum is lost so return to idle? TODO FIND OUT
                    return ANIMATION_STEP.CLIMB_UP_IDLE;
            }
        }
        else if (nodeHeightDiff < 0)//negitive height diff, running down
        {
            if (futureNode == null)
                return ANIMATION_STEP.CLIMB_DOWN_IDLE;
            else
            {
                int futureNodeHeightDiff = futureNode.m_gridPos.y - nextNode.m_gridPos.y;
                if (nodeHeightDiff == 0) //Stright path just run
                    return ANIMATION_STEP.CLIMB_DOWN_RUN;
                else //All other case momentum is lost so return to idle? TODO FIND OUT
                    return ANIMATION_STEP.CLIMB_DOWN_IDLE;
            }
        }

        return ANIMATION_STEP.IDLE;
    }
}
