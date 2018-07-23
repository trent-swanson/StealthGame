using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GotoTarget", menuName = "AI Actions/GotoTarget")]
public class GotoTarget : AIAction {

	private bool attacked = false;
    private PlayerController targetPlayer;
 
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
    public override void Perform (Agent agent) {
		if (!agent.m_moving) {
			CalculatePath(agent);
			agent.m_actualTargetTile.target = true;
        }
        else {
            agent.Move(false);
        }
    }

    void CalculatePath(Agent agent) {
		Tile targetTile = agent.GetTargetTile(target);
		agent.FindPath(targetTile, false);
	}*/
}
