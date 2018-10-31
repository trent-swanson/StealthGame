using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "AI Actions/Melee Attack")]
public class MeleeAttack : AIAction
{
    private Agent m_target = null;
    //--------------------------------------------------------------------------------------
    // Initialisation of an action at node creation 
    // Setup any used varibles, can get varibles from parent
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //      If this action can continue, e.g. Goto requires a target set by its parent -> Patrol sets next waypoint
    //--------------------------------------------------------------------------------------
    public override bool ActionInit(NPC NPCAgent, AIAction parentAction)
    {
        m_target = NPCAgent.GetClosestTarget();
        NPCAgent.m_targetAgent = m_target;

        return (m_target != null);
        
    }

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void ActionStart(NPC NPCAgent)
    {
        List<NavNode> oneStep = new List<NavNode>();
        oneStep.Add(NPCAgent.m_currentNavNode);//Only need one step at a time
        NPCAgent.m_agentAnimationController.m_animationSteps = AnimationManager.GetNPCAnimationSteps(NPCAgent, oneStep, INTERACTION_TYPE.ATTACK);

        NPCAgent.m_agentAnimationController.PlayNextAnimation();
    }

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public override bool IsDone(NPC NPCAgent)
    {
        return NPCAgent.m_agentAnimationController.m_animationSteps.Count == 0;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(NPC NPCAgent)
    {
        NPCAgent.m_currentActionPoints -= m_baseActionCost;
        List<Agent> possibleTargets = NPCAgent.m_agentWorldState.GetPossibleTargets();
        possibleTargets.Remove(m_target);
        NPCAgent.m_agentWorldState.SetPossibleTargets(possibleTargets);
        m_target = null;

        NPCAgent.ToggleAlertIcon();

        //Get next patrol node
        NPCAgent.NextWaypoint();
    }


    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override bool Perform(NPC NPCAgent)
    {
        if(NPCAgent.m_agentAnimationController.m_playNextAnimation)
            NPCAgent.m_agentAnimationController.PlayNextAnimation();
        return true;
    }

    //--------------------------------------------------------------------------------------
    // Setups agents varibles to perform a given action.
    // e.g for got to patrol node, set the target node which goto node uses
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void SetUpChildVaribles(NPC NPCAgent)
    {
        if (m_target != null)
        {
            //Get closest adjacent node
            float closestDistanceSqr = Mathf.Infinity;
            NavNode targetNode = null;

            foreach (NavNode adjacentNavNode in m_target.m_currentNavNode.m_adjacentNodes)
            {
                float distanceSqr = Vector3.SqrMagnitude(adjacentNavNode.transform.position - NPCAgent.transform.position);
                if(distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    targetNode = adjacentNavNode;
                }
            }
            NPCAgent.m_targetNode = targetNode;
        }
    }
}
