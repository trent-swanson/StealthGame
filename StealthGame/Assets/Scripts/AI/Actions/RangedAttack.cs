using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "AI Actions/RangedAttack")]
public class RangedAttack : AIAction {

	private bool attacked = false;
 
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
        //get a list of all players in scene
        List<PlayerController> validPlayers =  new List<PlayerController>();
		//find all players in range with line of sight
		foreach(PlayerController player in GameObject.FindObjectsOfType(typeof(PlayerController))) {
			//is in range?
			if (Vector3.Distance(player.gameObject.transform.position, agent.transform.position) <= attackRange) {
				//check if we have line of sight
				RaycastHit hit;
				if (Physics.Raycast(agent.transform.position, player.gameObject.transform.position, out hit, attackRange)) {
					if (hit.transform.tag == "Player") {
						//we found a valid target
						validPlayers.Add(player);
					}
				}
			}
		}
        PlayerController closest = null;
        float closestDist = 0;
        //bool hasLineOfSight = false;

		if (validPlayers.Count > 0) {
			//find closest player
			foreach (PlayerController player in validPlayers) {
				if (closest == null) {
					if (player.m_knockedout) {
						validPlayers.Remove(player);
						continue;
					} else {
						// first one, so choose it for now, if it is not knockedout
						closest = player;
						closestDist = (player.gameObject.transform.position - agent.transform.position).magnitude;
					}
				} else {
					//check is player is knocked out
					if (player.m_knockedout) {
						validPlayers.Remove(player);
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
			if (closest != null) {
				target = closest.gameObject;
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
    }*/
}
