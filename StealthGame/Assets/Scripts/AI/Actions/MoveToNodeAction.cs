using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveToNode", menuName = "AIBehaviours/Actions/Move To Node")]
public class MoveToNodeAction : Action
{
    private NPCAgentPlanner m_NPCPlannerScript = null;
    private Vector3 m_targetPos = new Vector3(0.0f, 0.0f, 0.0f);
    private float m_speed = 1.0f;

    private bool m_atNode = false;

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void ActionInit(GameObject agent)
    {
        m_NPCPlannerScript = agent.GetComponent<NPCAgentPlanner>();
        NPCAgent NPCAgentScript = agent.GetComponent<NPCAgent>();
        if (m_NPCPlannerScript != null && NPCAgentScript != null)
        {
            Vector3 nodeExtents = m_NPCPlannerScript.TargetNode.GetComponent<BoxCollider>().size;
            m_targetPos = m_NPCPlannerScript.TargetNode.transform.position + (Vector3.up * nodeExtents.y/2.0f);

            m_speed = NPCAgentScript.m_speed;
        }
    }
    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public override bool IsDone(GameObject agent)
    {
        return m_atNode;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(GameObject agent)
    {
        m_atNode = false;
    }

    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void Perform(GameObject agent)
    {
        //TODO run off AStar

        Vector3 currentPos = agent.transform.position;
        Vector3 dir = m_targetPos - currentPos;

        Vector3 velocity = dir.normalized * Time.deltaTime * m_speed;

        if(Vector3.Magnitude(velocity) > Vector3.Magnitude(dir)) //Close to the target position
        {
            currentPos = m_targetPos;
            m_atNode = true;
        }
        else
            currentPos += velocity;

        agent.transform.position = currentPos;
    }

}
