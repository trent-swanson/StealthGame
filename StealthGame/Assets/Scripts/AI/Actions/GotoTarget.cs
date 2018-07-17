using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotoTarget", menuName = "AI Actions/GotoTarget")]
public class GotoTarget : Action {

	private bool attacked = false;
    private PlayerController targetPlayer;
 
    public float attackRange = 10;
 
    public GotoTarget () {
        AddPrecondition ("TargetLineOfSight", target);
        AddEffect ("AtTarget", target);
        AddEffect ("NearTarget", target);
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
		target = p_target;
		return true;
    }
     
    public override void Perform (Agent agent) {
		if (!agent.moving) {
			CalculatePath(agent);
			agent.actualTargetTile.target = true;
        }
        else {
            agent.Move(false);
        }
    }

    void CalculatePath(Agent agent) {
		Tile targetTile = agent.GetTargetTile(target);
		agent.FindPath(targetTile, false);
	}
}
