using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    //All possible animations
    public enum ANIMATION_STEP {IDLE, STEP, TURN_RIGHT, TURN_LEFT, TURN_AROUND, WALK, CLIMB_UP_IDLE, CLIMB_UP_WALK, CLIMB_DOWN_IDLE, CLIMB_DOWN_WALK, WALL_HIDE_RIGHT, WALL_HIDE_LEFT, ATTACK, WALL_ATTACK, RANGED_ATTACK, INTERACTABLE, PICKUP_ITEM, DEATH, REVIVE }//Animation states

    //--------------------------------------------------------------------------------------
    // Build player animtaion step list
    // 
    // Param
    //		agent: agent to build list for
    //		pathNodes: path of an agent to take if moving
    //		interactionType: if agent is performing na interaction, what is it
    //		interactionDir: what direction will interaction occur with, e.g puching may be facing north, punch requires southward direction
    // Return:
    //      animtaion step list
    //--------------------------------------------------------------------------------------
    public static List<ANIMATION_STEP> GetPlayerAnimationSteps(Agent agent, List<NavNode> pathNodes, INTERACTION_TYPE interactionType = INTERACTION_TYPE.NONE, FACING_DIR interactionDir = FACING_DIR.NONE)
    {
        List<ANIMATION_STEP> transitionSteps = new List<ANIMATION_STEP>();

        FACING_DIR agentDir = agent.m_facingDir;

        int pathCount = pathNodes.Count;

        if (pathCount == 2)//Moving one square
        {
            GetActionStepsForSingleStep(ref agentDir, transitionSteps, pathNodes[0], pathNodes[1]);
        }
        else if(pathCount > 2)//Normal movement
        {
            for (int i = 0; i < pathCount - 2; i++)//Create all steps between, only will be movement
            {
                GetActionStepsForRunning(ref agentDir, transitionSteps, pathNodes[i], pathNodes[i + 1], pathNodes[i + 2]);
            }

            GetActionStepsForRunning(ref agentDir, transitionSteps, pathNodes[pathCount - 2], pathNodes[pathCount - 1]);//Last step to add
        }

        if (interactionType == INTERACTION_TYPE.ATTACK || interactionType == INTERACTION_TYPE.REVIVE)
            interactionDir = Agent.GetFacingDir((agent.m_targetAgent.transform.position - pathNodes[pathCount - 1].m_nodeTop).normalized);
        else if (interactionType == INTERACTION_TYPE.WALL_ATTACK)
            interactionDir = FACING_DIR.NONE;
        else if (interactionType == INTERACTION_TYPE.INTERACTABLE)
        {
            Vector3 interactDir = agent.m_targetInteractable.transform.position - pathNodes[pathCount - 1].transform.position;
            interactDir.y = 0;
            interactionDir = Agent.GetFacingDir(interactDir.normalized);
        }

        GetInteraction(ref agentDir, interactionDir, transitionSteps, interactionType);

        return transitionSteps;
    }

    //--------------------------------------------------------------------------------------
    // Build NPC animtaion step list
    // Cut down version of GetPlayerAnimationSteps, doesnt encounter need for interation dir
    // 
    // Param
    //		agent: agent to build list for
    //		pathNodes: path of an agent to take if moving
    //		interactionType: if agent is performing na interaction, what is it
    // Return:
    //      animtaion step list
    //--------------------------------------------------------------------------------------
    public static List<ANIMATION_STEP> GetNPCAnimationSteps(Agent agent, List<NavNode> pathNodes, INTERACTION_TYPE interactionType = INTERACTION_TYPE.NONE)
    {
        List<ANIMATION_STEP> transitionSteps = new List<ANIMATION_STEP>();

        FACING_DIR agentDir = agent.m_facingDir;

        int pathCount = pathNodes.Count;

        for (int i = 0; i < pathCount - 1; i++)//Create all steps between, only will be movement
        {
            FACING_DIR nextDir = Agent.GetFacingDir(pathNodes[i + 1].m_nodeTop - pathNodes[i].m_nodeTop);
            GetRotation(ref agentDir, nextDir, ref transitionSteps);
            agentDir = nextDir;

            transitionSteps.Add(ANIMATION_STEP.WALK);
        }

        FACING_DIR interactionDir = FACING_DIR.NONE;

        if (interactionType == INTERACTION_TYPE.ATTACK || interactionType == INTERACTION_TYPE.REVIVE)
            interactionDir = Agent.GetFacingDir((agent.m_targetAgent.transform.position - pathNodes[pathCount - 1].m_nodeTop).normalized);

        GetInteraction(ref agentDir, interactionDir, transitionSteps, interactionType);

        List<ANIMATION_STEP> singleStep = new List<ANIMATION_STEP>();
        singleStep.Add(transitionSteps[0]);
        return singleStep;
    }

    //--------------------------------------------------------------------------------------
    // Animaiton for a single step, excludes need to add idle to end of animation list
    // 
    // Param
    //		playerDir: ref to agent facing dir, updates as needed
    //		transitionSteps: steps to perform for desired animaiton list
    //		currentNode: current node of animaiton list
    //		nextNode: what the next node will be
    //--------------------------------------------------------------------------------------
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

    //--------------------------------------------------------------------------------------
    // Animaiton for a running movement, requires need to add idle to end of animation list
    // 
    // Param
    //		playerDir: ref to agent facing dir, updates as needed
    //		transitionSteps: steps to perform for desired animaiton list
    //		currentNode: current node of animaiton list
    //		nextNode: what the next node will be
    //      futureNode: on null future node, require idle to be used to go from running to stop
    //--------------------------------------------------------------------------------------
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

    //--------------------------------------------------------------------------------------
    // Build animaiton list for an interaction
    // 
    // Param
    //		playerDir: ref to agent facing dir, updates as needed
    //      interactionDir: if interaction requires a given direction, modify rotation to face this way
    //		transitionSteps: steps to perform for desired animaiton list
    //		interactionType: what the interaction will be, alters what aniamtion step is added
    //--------------------------------------------------------------------------------------
    private static void GetInteraction(ref FACING_DIR playerDir, FACING_DIR interactionDir, List<ANIMATION_STEP> transitionSteps, INTERACTION_TYPE interactionType = INTERACTION_TYPE.NONE)
    {
        if(interactionDir!= FACING_DIR.NONE)
            GetRotation(ref playerDir, interactionDir, ref transitionSteps);

        switch (interactionType)
        {
            case INTERACTION_TYPE.ATTACK:
                transitionSteps.Add(ANIMATION_STEP.ATTACK);
                break;
            case INTERACTION_TYPE.WALL_ATTACK:
                transitionSteps.Add(ANIMATION_STEP.WALL_ATTACK);
                break;
            case INTERACTION_TYPE.REVIVE:
                transitionSteps.Add(ANIMATION_STEP.REVIVE);
                break;
            case INTERACTION_TYPE.INTERACTABLE:
                transitionSteps.Add(ANIMATION_STEP.INTERACTABLE);
                break;
            case INTERACTION_TYPE.PICKUP_ITEM:
                transitionSteps.Add(ANIMATION_STEP.PICKUP_ITEM);
                break;
            case INTERACTION_TYPE.NONE:
            default:
                break;
        }
    }

    //--------------------------------------------------------------------------------------
    // On player turn end, find closest wall to hide to, if not null, this is called.
    // 
    // Param
    //		wallHideDir: wall to hide against, effect rotation
    //      agent: agent to perform hding action to
    //--------------------------------------------------------------------------------------
    public static List<ANIMATION_STEP> EndTurnWallHide(FACING_DIR wallHideDir, Agent agent)
    {
        List<ANIMATION_STEP> transitionSteps = new List<ANIMATION_STEP>();

        FACING_DIR agentDir = agent.m_facingDir;
        GetRotation(ref agentDir, wallHideDir, ref transitionSteps);

        transitionSteps.Add(ANIMATION_STEP.WALL_HIDE_RIGHT);

        return transitionSteps;
    }

    //--------------------------------------------------------------------------------------
    // Rotate towards a given direction
    // 
    // Param
    //		currentDir: current facing direction, modified as needed
    //      nextDir: the direction agent is required to face
    //      transitionSteps: steps to perform for desired animaiton list
    //--------------------------------------------------------------------------------------
    public static void GetRotation(ref FACING_DIR currentDir, FACING_DIR nextDir, ref List<ANIMATION_STEP> transitionSteps)
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
                    transitionSteps.Add(ANIMATION_STEP.TURN_AROUND);
                    break;
                default:
                    break;
            }

            currentDir = nextDir;
        }
    }


}
