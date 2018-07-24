using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol", menuName = "AI Actions/Patrol")]
public class Patrol : AIAction {

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void ActionInit(Agent agent)
    {

    }

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public override bool IsDone(Agent agent)
    {
        return true;
    }

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void EndAction(Agent agent)
    {

    }


    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public override void Perform(Agent agent)
    {
        List<GameObject> waypoints = agent.GetComponent<NPC>().m_waypoints;
        Vector3 target = waypoints[agent.GetComponent<NPC>().m_currentWaypoint].transform.position;

        Vector3 currentPos = agent.transform.position;
        Vector3 dir = target - currentPos;

        Vector3 velocity = dir.normalized * Time.deltaTime * agent.m_moveSpeed;

        if (Vector3.Magnitude(velocity) > Vector3.Magnitude(dir))
        {
            agent.transform.position = target;

            agent.GetComponent<NPC>().m_currentWaypoint++;
            if (agent.GetComponent<NPC>().m_currentWaypoint >= waypoints.Count)
            {
                agent.GetComponent<NPC>().m_currentWaypoint = 0;
            }
        }
    }
}
