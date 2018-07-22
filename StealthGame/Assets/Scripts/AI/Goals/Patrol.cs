using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol", menuName = "AI Goals/Patrol")]
public class Patrol : AIAction {
	public HashSet<KeyValuePair<string,object>> desiredWorldState = new HashSet<KeyValuePair<string, object>> ();
	public int priority = 1;
 
    public Patrol () {
        AddPrecondition ("Patrolling", true);
    }
     
     
    public override void Reset () {}
     
    public override bool IsDone () {
		return true;
	}
     
    public override bool RequiresInRange () {
        return true;
    }
     
    public override bool CheckProceduralPrecondition (GameObject agent, GameObject p_target) {
		return true;
    }
     
    public override void Perform (Agent agent) {}
}

