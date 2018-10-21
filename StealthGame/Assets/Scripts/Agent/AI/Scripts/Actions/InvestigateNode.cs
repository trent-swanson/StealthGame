using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InvestigateNode", menuName = "AI Actions/Investigate Node")]
public class InvestigateNode : AIAction
{
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
        return NPCAgent.m_agentWorldState.GetInvestigationNodes().Count > 0;
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
        return true;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(NPC NPCAgent)
    {
        List<NPC.InvestigationNode> investigationNodes = NPCAgent.m_agentWorldState.GetInvestigationNodes();
        investigationNodes.RemoveAt(0);
        NPCAgent.m_agentWorldState.SetInvestigationNodes(investigationNodes);

        NPCAgent.ToggleAlertIcon();
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
        List<NPC.InvestigationNode> investigationNodes = NPCAgent.m_agentWorldState.GetInvestigationNodes();
        if (investigationNodes.Count > 0)
        {
            NPCAgent.m_targetNode = investigationNodes[0].m_node;
        }
        else
        {
            NPCAgent.m_targetNode = null;
        }
    }
}
