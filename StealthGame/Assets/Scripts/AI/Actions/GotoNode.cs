using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotoNode", menuName = "AI Actions/GotoNode")]
public class GotoNode : Action {

	private bool attacked = false;
    private PlayerController targetPlayer;
 
    public float attackRange = 10;
 
    public GotoNode () {
        AddEffect ("AtNode", target);
        AddEffect ("Patrolling", true);
    }
     
     
    public override void Reset () {
        attacked = false;
        targetPlayer = null;
    }
     
    public override bool IsDone () {
        return attacked;
    }
     
    public override bool RequiresInRange () {
        return false;
    }
     
    public override bool CheckProceduralPrecondition (GameObject agent, GameObject p_target) {
		if (p_target != null) {
			target = p_target;
		} else if (agent.GetComponent<NPC>()) {
			NPC npc = agent.GetComponent<NPC>();
			target = npc.waypoints[npc.currentWaypoint];
		}
		return true;
    }
     
    public override void Perform (Agent agent) {
        Debug.Log("Perform");
		if (!agent.moving) {
			NextWaypoint(agent);
			CalculatePath(agent);
			agent.actualTargetTile.target = true;
        }
        else {
            agent.Move(false);
        }
    }

    void CalculatePath(Agent agent) {
        NPC npc = agent.GetComponent<NPC>();
        if (npc.target == null) {
            Tile targetTile = agent.GetTargetTile(npc.waypoints[npc.currentWaypoint]);
		    agent.FindPath(targetTile, true);
        } else {
           Tile targetTile = agent.GetTargetTile(target);
		    agent.FindPath(targetTile, true);
        }
        agent.moving = true;
	}

	void NextWaypoint(Agent agent) {
        List<GameObject> waypoints = agent.GetComponent<NPC>().waypoints;
		if (Vector3.Distance(agent.transform.position, waypoints[agent.GetComponent<NPC>().currentWaypoint].transform.position) < 0.15f) {
			agent.GetComponent<NPC>().currentWaypoint++;
			if (agent.GetComponent<NPC>().currentWaypoint >= waypoints.Count) {
				agent.GetComponent<NPC>().currentWaypoint = 0;
			}
		}
		target = waypoints[agent.GetComponent<NPC>().currentWaypoint];
	}
}
