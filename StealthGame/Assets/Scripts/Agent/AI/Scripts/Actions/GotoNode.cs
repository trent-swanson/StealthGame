using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotoNode", menuName = "AI Actions/Go to Node")]
public class GotoNode : AIAction
{
    private bool m_isDone = false;

    private NavNode m_targetNode = null;

    private Navigation m_navigation = null;

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
        m_navigation = GameObject.FindGameObjectWithTag("GameController").GetComponent<Navigation>();

        if (parentAction!=null)
        {
            parentAction.SetUpChildVaribles(NPCAgent);
            m_targetNode = NPCAgent.m_targetNode;
            if(m_targetNode!=null)
                return true;
        }
        return false;
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
        List<NavNode> navPath = new List<NavNode>();
        
        m_isDone = false;
        if (NPCAgent.m_currentNavNode != null)
        {
            navPath = m_navigation.GetNavPath(NPCAgent.m_currentNavNode, m_targetNode, NPCAgent);

            if(navPath.Count == 0)//Unable to reach navnode, attempt to get to adjacent node
            {
                navPath = GetShortestPath(NPCAgent.m_currentNavNode, ref m_targetNode, NPCAgent);

                if (navPath.Count == 0)//unable to find any path, return
                {
                    return;
                }
            }

            NPCAgent.m_path.Clear();
            NPCAgent.m_path = navPath;

            List<NavNode> oneTurnSteps = new List<NavNode>();
            oneTurnSteps.Add(navPath[0]);//Only need one step at a time
            oneTurnSteps.Add(navPath[1]);
            NPCAgent.m_agentAnimationController.m_animationSteps = AnimationManager.GetAnimationSteps(NPCAgent, oneTurnSteps);

            NPCAgent.m_agentAnimationController.PlayNextAnimation();
        }
    }

    //--------------------------------------------------------------------------------------
    // Get shortest path to adjacent nodes
    // 
    // Param
    //		startingNode: the agents current node
    //		targetNode: new target node
    // Return:
    //      New shortest path to closest adjacent node
    //--------------------------------------------------------------------------------------
    private List<NavNode> GetShortestPath(NavNode startingNode, ref NavNode targetNode, Agent agent)
    {
        NavNode newTargetNode = null;
        List<NavNode> shortestPath = new List<NavNode>();
        float distance = Mathf.Infinity;

        foreach (NavNode adjacentNode in targetNode.m_adjacentNodes)
        {
            List<NavNode> tempPath = m_navigation.GetNavPath(startingNode, adjacentNode, agent);
            if(tempPath.Count != 0 && tempPath.Count < distance)
            {
                distance = tempPath.Count;
                shortestPath = tempPath;
                newTargetNode = adjacentNode;
            }
        }

        if(newTargetNode!= null)
            targetNode = newTargetNode;
        return shortestPath;
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
        return m_isDone;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(NPC NPCAgent)
    {

    }

    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return
    //      If perform can no longer function return false
    //--------------------------------------------------------------------------------------
    public override bool Perform(NPC NPCAgent)
    {
        if(NPCAgent.m_agentAnimationController.m_playNextAnimation)//Arrived at node
        {
            //Precheck for aniomation step count
            if (NPCAgent.m_agentAnimationController.m_animationSteps.Count == 0)
                m_isDone = true;
            else
                NPCAgent.m_agentAnimationController.PlayNextAnimation();
        }

        return true;
    }

    //--------------------------------------------------------------------------------------
    // Setups agents varibles to perform a given action.
    // e.g for got to patrol node, set the target node which goto node uses
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void SetUpChildVaribles(NPC NPCAgent) { }
}
