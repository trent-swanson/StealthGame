using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : ScriptableObject {

	[Tooltip("What states this acition will satisfy")]
    [SerializeField]
    public List<WorldState.WORLD_STATE> m_satisfiedWorldStates = new List<WorldState.WORLD_STATE>();

    [Tooltip("States this action requires")]
    [SerializeField]
    public List<WorldState.WORLD_STATE> m_requiredWorldStates = new List<WorldState.WORLD_STATE>();

    [Tooltip("Cost of action")]
    [SerializeField]
    public int m_actionCost = 0;

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void ActionInit(Agent agent);

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public abstract bool IsDone(Agent agent);

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void EndAction(Agent agent);

    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void Perform(Agent agent);
}