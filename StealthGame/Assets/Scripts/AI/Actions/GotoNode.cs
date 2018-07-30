using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotoNode", menuName = "AI Actions/GotoNode")]
public class GotoNode : AIAction
{
    private List<NavNode> m_navPath = new List<NavNode>();
    private bool m_isDone = false;

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void ActionInit(NPC NPCAgent)
    {
        m_isDone = false;
        if (NPCAgent.m_currentNavNode != null)
            m_navPath = Navigation.Instance.GetNavPath(NPCAgent.m_currentNavNode, NPCAgent.m_agentWorldState.m_targetNode);
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
    //--------------------------------------------------------------------------------------
    public override void Perform(NPC NPCAgent)
    {
        if(m_navPath.Count == 0)
        {
            m_isDone = true;
            return;
        }

        Vector3 velocityVector = m_navPath[0].transform.position - NPCAgent.transform.position;
        float translateDis = velocityVector.magnitude;

        velocityVector = velocityVector.normalized * Time.deltaTime * NPCAgent.m_moveSpeed;

        if(velocityVector.magnitude > translateDis)//Arrived at node
        {
            NPCAgent.transform.position = m_navPath[0].transform.position;
            NPCAgent.m_currentNavNode = m_navPath[0];
            m_navPath.RemoveAt(0);
            m_isDone = true;
        }
        else
        {
            NPCAgent.transform.position += velocityVector;
        }
    }
}
