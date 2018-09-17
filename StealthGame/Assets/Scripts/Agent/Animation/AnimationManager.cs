using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum ANIMATION_STEP {IDLE, STEP, TURN_RIGHT, TURN_LEFT, TURN_AROUND, WALK, CLIMB_UP_IDLE, CLIMB_UP_WALK, CLIMB_DOWN_IDLE, CLIMB_DOWN_WALK, WALL_HIDE_RIGHT, WALL_HIDE_LEFT, ATTACK, RANGED_ATTACK, INTERACTION, PICKUP_ITEM_GROUND, PICKUP_ITEM_SHELF, DEATH, REVIVE }//Animation states

    public static List<ANIMATION_STEP> GetAnimationSteps(Agent agent, List<NavNode> pathNodes, INTERACTION_TYPE interactionType = INTERACTION_TYPE.NONE, FACING_DIR interactionDir = FACING_DIR.NONE)
    {
        List<ANIMATION_STEP> transitionSteps = new List<ANIMATION_STEP>();

        FACING_DIR playerDir = agent.m_facingDir;

        int pathCount = pathNodes.Count;

        if (pathCount == 2)//Moving one square
        {
            GetActionStepsForSingleStep(ref playerDir, transitionSteps, pathNodes[0], pathNodes[1]);
        }
        else if(pathCount > 2)            //Normal movement
        {
            for (int i = 0; i < pathCount - 2; i++)//Create all steps between, only will be movement
            {
                GetActionStepsForRunning(ref playerDir, transitionSteps, pathNodes[i], pathNodes[i + 1], pathNodes[i + 2]);
            }

            GetActionStepsForRunning(ref playerDir, transitionSteps, pathNodes[pathCount - 2], pathNodes[pathCount - 1]);//Last step to add
        }

        if (interactionType == INTERACTION_TYPE.ATTACK)
            interactionDir = Agent.GetFacingDir((agent.m_targetAgent.transform.position - pathNodes[pathCount - 1].m_nodeTop).normalized);
        else if (interactionType == INTERACTION_TYPE.REVIVE)
            interactionDir = Agent.GetFacingDir((agent.m_targetAgent.transform.position - pathNodes[pathCount - 1].m_nodeTop).normalized);

        GetInteraction(ref playerDir, interactionDir, transitionSteps, interactionType);
        agent.m_facingDir = playerDir;

        return transitionSteps;
    }

    private static void GetActionStepsForSingleStep(ref FACING_DIR playerDir, List<ANIMATION_STEP> transitionSteps, NavNode currentNode, NavNode nextNode)
    {
        FACING_DIR nextDir = Agent.GetFacingDir(nextNode.m_nodeTop - currentNode.m_nodeTop);
        GetRotation(ref playerDir, nextDir, ref transitionSteps);
        playerDir = nextDir;

        int nodeHeightDiff = nextNode.m_gridPos.y - currentNode.m_gridPos.y;

        if (nodeHeightDiff == 0) //Stright path just run
        {
            transitionSteps.Add(ANIMATION_STEP.STEP);
        }
        else if (nodeHeightDiff > 0)//positive height diff, running up
        {
            transitionSteps.Add(ANIMATION_STEP.CLIMB_UP_IDLE);
        }
        else if (nodeHeightDiff < 0)//negitive height diff, running down
        {
            
            transitionSteps.Add(ANIMATION_STEP.CLIMB_DOWN_IDLE);
           
        }
    }

    private static void GetActionStepsForRunning(ref FACING_DIR playerDir, List<ANIMATION_STEP> transitionSteps, NavNode currentNode, NavNode nextNode, NavNode futureNode = null)
    {
        FACING_DIR nextDir = Agent.GetFacingDir(nextNode.m_nodeTop - currentNode.m_nodeTop);
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

    private static void GetInteraction(ref FACING_DIR playerDir, FACING_DIR interactionDir, List<ANIMATION_STEP> transitionSteps, INTERACTION_TYPE interactionType = INTERACTION_TYPE.NONE)
    {
        GetRotation(ref playerDir, interactionDir, ref transitionSteps);

        switch (interactionType)
        {
            case INTERACTION_TYPE.WALL_HIDE: // TODO left right
                transitionSteps.Add(ANIMATION_STEP.WALL_HIDE_RIGHT);
                break;
            case INTERACTION_TYPE.USE_OBJECT:
                transitionSteps.Add(ANIMATION_STEP.INTERACTION);
                break;
            case INTERACTION_TYPE.ATTACK:
                transitionSteps.Add(ANIMATION_STEP.ATTACK);
                break;
            case INTERACTION_TYPE.REVIVE:
                transitionSteps.Add(ANIMATION_STEP.REVIVE);
                break;
            case INTERACTION_TYPE.PICKUP_ITEM_GROUND:
                transitionSteps.Add(ANIMATION_STEP.PICKUP_ITEM_GROUND);
                break;
            case INTERACTION_TYPE.NONE:
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

            currentDir = nextDir;
        }
    }


}
