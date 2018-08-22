using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public enum ANIMATION_STEP {IDLE, STEP, TURN_RIGHT, TURN_LEFT, RUN, CLIMB_UP_IDLE, CLIMB_UP_RUN, CLIMB_DOWN_IDLE, CLIMB_DOWN_RUN, WALL_HIDE_RIGHT, WALL_HIDE_LEFT, ATTACK, RANGED_ATTACK, INTERACTION }//Animation states

    public enum FACING_DIR {NORTH, EAST, SOUTH, WEST }


    public static List<ANIMATION_STEP> GetAnimationSteps(Agent agent, List<NavNode> pathNodes, PlayerActions.INTERACTION_TYPE interactionType = PlayerActions.INTERACTION_TYPE.NONE)
    {
        List<ANIMATION_STEP> transitionSteps = new List<ANIMATION_STEP>();

        FACING_DIR playerFacing = GetFacingDir(agent.transform.forward);

        int pathCount = pathNodes.Count;

        if (pathCount < 2)//Not moving, still needs to interact
        {
            GetInteraction(transitionSteps, interactionType);
            return transitionSteps;
        }
        else if (pathCount == 2)//Moving one square
        {
            GetActionSteps(ref playerFacing, transitionSteps, pathNodes[0], pathNodes[1]);

            GetInteraction(transitionSteps, interactionType);
            return transitionSteps;
        }

        //Normal movement
        for (int i = 0; i < pathCount - 2; i++)//Create all steps between, only will be movement
        {
            GetActionSteps(ref playerFacing, transitionSteps,  pathNodes[i], pathNodes[i + 1], pathNodes[i + 2]);
        }

        GetActionSteps(ref playerFacing, transitionSteps, pathNodes[pathCount - 2], pathNodes[pathCount - 1]);//Last step to add

        if (interactionType != PlayerActions.INTERACTION_TYPE.NONE)//End of path is an interactable
        {
            GetInteraction(transitionSteps, interactionType);
        }

        return transitionSteps;
    }

    private static void GetActionSteps(ref FACING_DIR playerDir, List<ANIMATION_STEP> transitionSteps, NavNode currentNode, NavNode nextNode, NavNode futureNode = null)
    {
        FACING_DIR nextDir = GetFacingDir(nextNode.m_nodeTop - currentNode.m_nodeTop);
        GetRotation(ref transitionSteps, playerDir, nextDir);
        playerDir = nextDir;

        int nodeHeightDiff = nextNode.m_gridPos.y - currentNode.m_gridPos.y;

        if (nodeHeightDiff == 0) //Stright path just run
        {
            if (futureNode == null)
                transitionSteps.Add(ANIMATION_STEP.IDLE);
            else
                transitionSteps.Add(ANIMATION_STEP.RUN);
        }
        else if (nodeHeightDiff > 0)//positive height diff, running up
        {
            if(futureNode==null)
                transitionSteps.Add(ANIMATION_STEP.CLIMB_UP_IDLE);
            else
            {
                int futureNodeHeightDiff = futureNode.m_gridPos.y - nextNode.m_gridPos.y;
                if (futureNodeHeightDiff == 0) //Stright path just run
                    transitionSteps.Add(ANIMATION_STEP.CLIMB_UP_RUN);
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
                    transitionSteps.Add(ANIMATION_STEP.CLIMB_DOWN_RUN);
                else //All other case momentum is lost so return to idle? TODO FIND OUT
                    transitionSteps.Add(ANIMATION_STEP.CLIMB_DOWN_IDLE);
            }
        }
    }

    private static void GetInteraction(List<ANIMATION_STEP> transitionSteps, PlayerActions.INTERACTION_TYPE interactionType = PlayerActions.INTERACTION_TYPE.NONE)
    {
        switch (interactionType)
        {
            case PlayerActions.INTERACTION_TYPE.WALL_HIDE: // TODO left right
                transitionSteps.Add(ANIMATION_STEP.WALL_HIDE_RIGHT);
                break;
            case PlayerActions.INTERACTION_TYPE.USE_OBJECT:
                transitionSteps.Add(ANIMATION_STEP.INTERACTION);
                break;
            case PlayerActions.INTERACTION_TYPE.ATTACK:
                transitionSteps.Add(ANIMATION_STEP.ATTACK);
                break;
            case PlayerActions.INTERACTION_TYPE.NONE:
            default:
                break;
        }
    }

    private static void GetRotation(ref List<ANIMATION_STEP> transitionSteps, FACING_DIR currentDir, FACING_DIR nextDir)
    {
        if (currentDir != nextDir)
        {
            int dirAmount = (int)currentDir - (int)nextDir;

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
    }

    private static FACING_DIR GetFacingDir(Vector3 dir)
    {
        float angle = Vector3.SignedAngle(dir.normalized, new Vector3(0, 0, 1), Vector3.up);

        if (angle < 10.0f && angle > -10.0f) //allows for minor inaccuracies
            return FACING_DIR.NORTH;
        if (angle > 170.0f || angle < -170.0f)
            return FACING_DIR.SOUTH;
        if(angle < 100 && angle > 70)
            return FACING_DIR.EAST;
        if (angle < -70 && angle > -100)
            return FACING_DIR.WEST;
        return FACING_DIR.NORTH;
    }
}
