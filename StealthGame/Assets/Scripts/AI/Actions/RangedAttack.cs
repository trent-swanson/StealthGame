using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "AI Actions/RangedAttack")]
public class RangedAttack : Action {

	private bool attacked = false;
    private PlayerController targetPlayer;
 
    public float attackRange = 10;
 
    public RangedAttack () {
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
        return true;
    }
     
    public override bool CheckProceduralPrecondition (GameObject agent, GameObject p_target) {
		target = p_target;
        //get a list of all players in scene
        List<PlayerController> players =  new List<PlayerController>();
		foreach(PlayerController player in GameObject.FindObjectsOfType(typeof(PlayerController))) { 
        	players.Add(player);
		}
        PlayerController closest = null;
        float closestDist = 0;
        bool hasLineOfSight = false;

		//continue until we have line of sight on a player
		while (!hasLineOfSight) {
			//find closest player
			foreach (PlayerController player in players) {
				if (closest == null) {
					if (player.knockedout) {
						players.Remove(player);
						continue;
					} else {
						// first one, so choose it for now, if it is not knockedout
						closest = player;
						closestDist = (player.gameObject.transform.position - agent.transform.position).magnitude;
					}
				} else {
					//check is player is knocked out
					if (player.knockedout) {
						players.Remove(player);
						continue;
					} else {
						// is this one closer than the last?
						float dist = (player.gameObject.transform.position - agent.transform.position).magnitude;
						if (dist < closestDist) {
							// we found a closer one, use it
							closest = player;
							closestDist = dist;
						}
					}
				}
			}
			targetPlayer = closest;

			//check if closest player is within attack range
			if (Vector3.Distance(targetPlayer.gameObject.transform.position, agent.transform.position) <= attackRange) {
				//check if we have line of sight on closest player
				RaycastHit hit;
				if (Physics.Raycast(agent.transform.position, targetPlayer.gameObject.transform.position, out hit, attackRange)) {
					if (hit.transform.tag == "Player") {
						//we found a valid target
						target = targetPlayer.gameObject;
						return true;
					} else {
						players.Remove(targetPlayer);
						continue;
					}
				}
			} else {
				//if closest player greater than attack range, no need to keep looking, return false
				return false;
			}
		}
        return false;
    }
     
    public override void Perform (Agent agent) {
		Debug.Log("Shot Player: " + target.name);
    }
}
