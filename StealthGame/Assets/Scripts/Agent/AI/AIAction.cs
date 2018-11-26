using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction : ScriptableObject {

	[Tooltip("What states this acition will satisfy")]
    [SerializeField]
    public List<WorldState.WORLD_STATE> m_satisfiedWorldStates = new List<WorldState.WORLD_STATE>();

    [System.Serializable]
    public struct RequiredWorldState
    {
        public WorldState.WORLD_STATE m_worldState;

        public enum PRIORITY {NONE, LOW, MEDIUM, HIGH, VERY_HIGH };
        public PRIORITY m_priority;
    }

    public enum ACTION_VALIDITY { }

    [Tooltip("States this action requires")]
    [SerializeField]
    public List<RequiredWorldState> m_requiredWorldStates = new List<RequiredWorldState>();

    [Tooltip("Cost of action")]
    [SerializeField]
    public int m_baseActionCost = 0; //How much each step will take in an action

    //--------------------------------------------------------------------------------------
    // Initialisation of an action at node creation 
    // Setup any used varibles, can get varibles from parent
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    // Return:
    //      If this action can continue, e.g. Goto requires a target set by its parent -> Patrol sets next waypoint
    //--------------------------------------------------------------------------------------
    public abstract bool ActionInit(NPC NPCAgent, AIAction parentAction);

    //--------------------------------------------------------------------------------------
    // Initialisation of an action 
    // Runs once when action starts from the list
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void ActionStart(NPC NPCAgent);

    //--------------------------------------------------------------------------------------
    // Has the action been completed
    // 
    // Param
    //		agent: Gameobject which script is used on
    // Return:
    //		Is all action moves have been completed
    //--------------------------------------------------------------------------------------
    public abstract bool IsDone(NPC NPCAgent);

    //--------------------------------------------------------------------------------------
    // Agent Has been completed, clean up anything that needs to be
    // 
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void EndAction(NPC NPCAgent);

    //--------------------------------------------------------------------------------------
    // Perform actions effects, e.g. Moving towards opposing agent
    // Should happen on each update
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract bool Perform(NPC NPCAgent);

    //--------------------------------------------------------------------------------------
    // Setups agents varibles to perform a given action.
    // e.g for got to patrol node, set the target node which goto node uses
    //
    // Param
    //		NPCAgent: Gameobject which script is used on
    //--------------------------------------------------------------------------------------
    public abstract void SetUpChildVaribles(NPC NPCAgent);
}