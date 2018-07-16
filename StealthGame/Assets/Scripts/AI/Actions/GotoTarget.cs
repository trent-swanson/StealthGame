using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoTarget : Action {

	private bool attacked = false;
    private PlayerController targetPlayer;
 
    public float attackRange = 10;
 
    public GotoTarget () {
        AddPrecondition ("NearTarget", target);
        AddPrecondition ("TargetLineOfSight", target);
        AddEffect ("AttackingTarget", target);
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
     
    public override bool Perform (GameObject agent) {
		Debug.Log("Shot Player: " + target.name);
        return true;
    }
}
