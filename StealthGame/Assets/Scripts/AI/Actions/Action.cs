using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    [Tooltip("What states this acition will satisfy")]
    [SerializeField]
    private List<WorldState.WORLD_STATE> m_satisfiedWorldStates = new List<WorldState.WORLD_STATE>();
    public List<WorldState.WORLD_STATE> SatisfiedWorldStates
    {
        get { return m_satisfiedWorldStates; }
    }

    [Tooltip("States this action requires")]
    [SerializeField]
    private List<WorldState.WORLD_STATE> m_requiredWorldStates = new List<WorldState.WORLD_STATE>();
    public List<WorldState.WORLD_STATE> RequiredWorldStates
    {
        get { return m_requiredWorldStates; }
    }

    [Tooltip("Cost of action")]
    [SerializeField]
    private int m_actionCost = 0;
    public int ActionCost
    {
        get { return m_actionCost; }
    }

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void ActionInit(GameObject agent);

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public abstract bool IsDone(GameObject agent);

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void EndAction(GameObject agent);

    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		agent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void Perform(GameObject agent);
}