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
        NPCAgent.m_attackingTarget = m_target;

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
        NPCAgent.m_currentNavNode.m_nodeType = NavNode.NODE_TYPE.WALKABLE;
        NPCAgent.m_currentNavNode.m_obstructingAgent = null;

        List<NavNode> oneStep = new List<NavNode>();
        oneStep.Add(NPCAgent.m_currentNavNode);//Only need one step at a time
        NPCAgent.m_agentAnimationController.m_animationSteps = AnimationManager.GetAnimationSteps(NPCAgent, oneStep, Agent.INTERACTION_TYPE.ATTACK);

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
        return NPCAgent.m_agentAnimationController.m_playNextAnimation;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(NPC NPCAgent)
    {
        NPCAgent.m_currentNavNode = m_target.m_currentNavNode;
        m_target = null;
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
            NPCAgent.m_targetNode = m_target.m_currentNavNode;
    }
}
