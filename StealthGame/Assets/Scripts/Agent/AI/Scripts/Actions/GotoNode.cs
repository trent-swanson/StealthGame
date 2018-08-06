using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotoNode", menuName = "AI Actions/Go to Node")]
public class GotoNode : AIAction
{
    private List<NavNode> m_navPath = new List<NavNode>();
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
            m_targetNode = NPCAgent.m_agentWorldState.m_targetNode;
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
        m_isDone = false;
        if (NPCAgent.m_currentNavNode != null)
        {
            m_navPath = m_navigation.GetNavPath(NPCAgent.m_currentNavNode, m_targetNode);
            NPCAgent.m_currentNavNode.m_nodeState = NavNode.NODE_STATE.UNSELECTED;
        }
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
        NPCAgent.m_currentNavNode.m_nodeState = NavNode.NODE_STATE.OBSTRUCTED;
    }


    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void Perform(NPC NPCAgent)
    {
        if (m_navPath.Count == 0 || m_navPath[0].m_nodeState == NavNode.NODE_STATE.OBSTRUCTED) //Return true when at at end or is occupied
        {
            m_isDone = true;
            return;
        }

        Vector3 targetPos = m_navPath[0].m_nodeTop;
        Vector3 velocityVector = targetPos - NPCAgent.transform.position;
        float translateDis = velocityVector.magnitude;

        velocityVector = velocityVector.normalized * Time.deltaTime * NPCAgent.m_moveSpeed;

        if(velocityVector.magnitude > translateDis)//Arrived at node
        {
            NPCAgent.transform.position = targetPos;
            NPCAgent.m_currentNavNode = m_navPath[0];
            m_navPath.RemoveAt(0);
            m_isDone = true;
        }
        else
        {
            NPCAgent.transform.position += velocityVector;
        }
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
