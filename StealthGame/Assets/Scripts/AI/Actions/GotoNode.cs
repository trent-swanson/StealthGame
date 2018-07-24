﻿using System.Collections;
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
        Tile targetTile = agent.GetTargetTile(agent.GetComponent<NPC>().m_target);
        agent.FindPath(targetTile, true);
        agent.GetComponent<NPC>().m_moving = true;
        agent.m_actualTargetTile.target = true;
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
        return !agent.GetComponent<NPC>().m_moving;
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
        agent.Move(false);
    }
}
