using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum ANIMATION_STEP {IDLE, STEP, TURN_RIGHT, TURN_LEFT, WALK, CLIMB_UP_IDLE, CLIMB_UP_WALK, CLIMB_DOWN_IDLE, CLIMB_DOWN_WALK, WALL_HIDE_RIGHT, WALL_HIDE_LEFT, ATTACK, RANGED_ATTACK, INTERACTION, DEATH }//Animation states

    public enum FACING_DIR {NORTH, EAST, SOUTH, WEST, NONE }

    public static List<ANIMATION_STEP> GetAnimationSteps(Agent agent, List<NavNode> pathNodes, Agent.INTERACTION_TYPE interactionType = Agent.INTERACTION_TYPE.NONE, FACING_DIR interactionDir = FACING_DIR.NONE)
    {
        List<ANIMATION_STEP> transitionSteps = new List<ANIMATION_STEP>();

        FACING_DIR playerDir = GetFacingDir(agent.transform.forward);
        if(interactionType == Agent.INTERACTION_TYPE.ATTACK)
            interactionDir = GetFacingDir((agent.m_attackingTarget.transform.position - agent.transform.position).normalized);

        int pathCount = pathNodes.Count;

        if (pathCount < 2)//Not moving, still needs to interact
        {
            GetInteraction(ref playerDir, interactionDir, transitionSteps, interactionType);
            return transitionSteps;
        }
        else if (pathCount == 2)//Moving one square
        {
            GetActionSteps(ref playerDir, transitionSteps, pathNodes[0], pathNodes[1]);

            GetInteraction(ref playerDir, interactionDir, transitionSteps, interactionType);
            return transitionSteps;
        }

        //Normal movement
        for (int i = 0; i < pathCount - 2; i++)//Create all steps between, only will be movement
        {
            GetActionSteps(ref playerDir, transitionSteps,  pathNodes[i], pathNodes[i + 1], pathNodes[i + 2]);
        }

        GetActionSteps(ref playerDir, transitionSteps, pathNodes[pathCount - 2], pathNodes[pathCount - 1]);//Last step to add

        GetInteraction(ref playerDir, interactionDir, transitionSteps, interactionType);

        return transitionSteps;
    }

    private static void GetActionSteps(ref FACING_DIR playerDir, List<ANIMATION_STEP> transitionSteps, NavNode currentNode, NavNode nextNode, NavNode futureNode = null)
    {
        FACING_DIR nextDir = GetFacingDir(nextNode.m_nodeTop - currentNode.m_nodeTop);
        GetRotation(ref playerDir, nextDir, ref transitionSteps);
        playerDir = nextDir;

        int nodeHeightDiff = nextNode.m_gridPos.y - currentNode.m_gridPos.y;

        if (nodeHeightDiff == 0) //Stright path just run
        {
            if (futureNode == null)
                transitionSteps.Add(ANIMATION_STEP.IDLE);
            else
                transitionSteps.Add(ANIMATION_STEP.WALK);
        }
        else if (nodeHeightDiff > 0)//positive height diff, running up
        {
            if(futureNode==null)
                transitionSteps.Add(ANIMATION_STEP.CLIMB_UP_IDLE);
            else
            {
                int futureNodeHeightDiff = futureNode.m_gridPos.y - nextNode.m_gridPos.y;
                if (futureNodeHeightDiff == 0) //Stright path just run
                    transitionSteps.Add(ANIMATION_STEP.CLIMB_UP_WALK);
                else //All other case momentum is lost so return to idle? TODO FIND OUT
                    transitionSteps.Add(ANIMATION_STEP.CLIMB_UP_IDLE);
            }
        }
        else if (nodeHeightDiff < 0)//negitive height diff, running down
        {
            if (futureNode == null)
                transitionSteps.Add(ANIMATION_STEP.CLIMB_DOWN_IDLE);
            else
            {
                int futureNodeHeightDiff = futureNode.m_gridPos.y - nextNode.m_gridPos.y;
                if (futureNodeHeightDiff == 0) //Stright path just run
                    transitionSteps.Add(ANIMATION_STEP.CLIMB_DOWN_WALK);
                else //All other case momentum is lost so return to idle? TODO FIND OUT
                    transitionSteps.Add(ANIMATION_STEP.CLIMB_DOWN_IDLE);
            }
        }
    }

    private static void GetInteraction(ref FACING_DIR playerDir, FACING_DIR interactionDir, List<ANIMATION_STEP> transitionSteps, Agent.INTERACTION_TYPE interactionType = Agent.INTERACTION_TYPE.NONE)
    {
        GetRotation(ref playerDir, interactionDir, ref transitionSteps);

        switch (interactionType)
        {
            case Agent.INTERACTION_TYPE.WALL_HIDE: // TODO left right
                transitionSteps.Add(ANIMATION_STEP.WALL_HIDE_RIGHT);
                break;
            case Agent.INTERACTION_TYPE.USE_OBJECT:
                transitionSteps.Add(ANIMATION_STEP.INTERACTION);
                break;
            case Agent.INTERACTION_TYPE.ATTACK:
                transitionSteps.Add(ANIMATION_STEP.ATTACK);
                break;
            case Agent.INTERACTION_TYPE.NONE:
            default:
                break;
        }
    }

    private static void GetRotation(ref FACING_DIR currentDir, FACING_DIR nextDir, ref List<ANIMATION_STEP> transitionSteps)
    {
        if (currentDir != nextDir && nextDir != FACING_DIR.NONE)
        {
            int dirAmount = (int)nextDir - (int)currentDir;

            switch (dirAmount)
            {
                case 1:
                case -3:
                    transitionSteps.Add(ANIMATION_STEP.TURN_RIGHT);
                    break;
                case -1:
                case 3:
                        transitionSteps.Add(ANIMATION_STEP.TURN_LEFT);
                    break;
                case 2:
                case -2:
                    transitionSteps.Add(ANIMATION_STEP.TURN_RIGHT);
                    transitionSteps.Add(ANIMATION_STEP.TURN_RIGHT);
                    break;
                default:
                    break;
            }
        }
        currentDir = nextDir;
    }

    private static FACING_DIR GetFacingDir(Vector3 dir)
    {
        dir.y = 0; //Dont need to use y 
        float angle = Vector3.SignedAngle(dir.normalized, new Vector3(0, 0, 1), Vector3.up);

        if (angle < 10.0f && angle > -10.0f) //allows for minor inaccuracies
            return FACING_DIR.NORTH;
        if (angle > 170.0f || angle < -170.0f)
            return FACING_DIR.SOUTH;
        if(angle < 100 && angle > 70)
            return FACING_DIR.WEST;
        if (angle < -70 && angle > -100)
            return FACING_DIR.EAST;
        return FACING_DIR.NONE;
    }
}
