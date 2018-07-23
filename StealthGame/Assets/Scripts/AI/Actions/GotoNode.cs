using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotoNode", menuName = "AI Actions/GotoNode")]
public class GotoNode : AIAction {
 
    public float attackRange = 10;
 
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
        return false;
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

    }

    /* 
    public override bool CheckProceduralPrecondition (GameObject agent, GameObject p_target) {
		if (p_target != null) {
			target = p_target;
		} else if (agent.GetComponent<NPC>()) {
			NPC npc = agent.GetComponent<NPC>();
			target = npc.m_waypoints[npc.m_currentWaypoint];
		}
		return true;
    }
     
    public override void Perform (Agent agent) {
        Debug.Log("Perform");
		if (!agent.m_moving) {
			NextWaypoint(agent);
			CalculatePath(agent);
			agent.m_actualTargetTile.target = true;
        }
        else {
            agent.Move(false);
        }
    }

    void CalculatePath(Agent agent) {
        NPC npc = agent.GetComponent<NPC>();
        if (npc.m_target == null) {
            Tile targetTile = agent.GetTargetTile(npc.m_waypoints[npc.m_currentWaypoint]);
		    agent.FindPath(targetTile, true);
        } else {
           Tile targetTile = agent.GetTargetTile(target);
		    agent.FindPath(targetTile, true);
        }
        agent.m_moving = true;
	}

	void NextWaypoint(Agent agent) {
        List<GameObject> waypoints = agent.GetComponent<NPC>().m_waypoints;
		if (Vector3.Distance(agent.transform.position, waypoints[agent.GetComponent<NPC>().m_currentWaypoint].transform.position) < 0.15f) {
			agent.GetComponent<NPC>().m_currentWaypoint++;
			if (agent.GetComponent<NPC>().m_currentWaypoint >= waypoints.Count) {
				agent.GetComponent<NPC>().m_currentWaypoint = 0;
			}
		}
		target = waypoints[agent.GetComponent<NPC>().m_currentWaypoint];
	}*/
}
