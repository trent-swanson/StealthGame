using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GOAP : MonoBehaviour
{
    public enum GOAP_UPDATE_STATE {PERFORMING, INVALID, COMPLETED}
    private NPC m_NPC = null;

    [Space]
    public List<Goal> m_ambientGoals = new List<Goal>();
    public List<Goal> m_suspiciousGoals = new List<Goal>();
    public List<Goal> m_alertGoals = new List<Goal>();

    public List<AIAction> m_possibleActions = new List<AIAction>();

    [Space]
    public List<AIAction> m_actionList = new List<AIAction>();
    
    public enum GOAL_STATE { AMBIENT, SUSPICIOUS, ALERT }
    public GOAL_STATE m_goalState;

    private List<Goal> m_goalList = new List<Goal>();

    private void Start()
    {
        m_NPC = GetComponent<NPC>();
        m_goalState = GOAL_STATE.AMBIENT;
    }

    //setting up GOAP usage this action
    //Return : false when unable to find any valid action
    public bool GOAPInit()
    {
        GetGoalPriority();
        StartCoroutine("ActionPlanning");

        if(m_actionList[0] != null && m_actionList[0].m_actionCost <= m_NPC.m_currentActionPoints)
        {
            m_currentAction.ActionStart(m_NPC);
            return true;
        }

        return false;
    }

    //GOAP frame by frame update
    //Return : true when action is completed
    public GOAP_UPDATE_STATE GOAPUpdate()
    {
        bool validAction = m_currentAction.Perform(m_NPC);

        if(!validAction)//Action was attempted but unable to complete
            return GOAP_UPDATE_STATE.INVALID;

        if (m_currentAction.IsDone(m_NPC))
        {
            m_currentAction.EndAction(m_NPC);
            return GOAP_UPDATE_STATE.COMPLETED;
        }
        return GOAP_UPDATE_STATE.PERFORMING;
    }

    public void GetGoalPriority()
    {
        //list of state goals
        //foreach goal DetermineGoalPriority() and add to a queue in order of cheapest
        //Find action plan for goal, if no plan found go to next goal
        //Tell Squad Manager what my goal is

        List<Goal> possibleGoals = new List<Goal>();

        //switch (m_goalState) //TODO add back
        //{
        //    case GOAL_STATE.AMBIENT:
        //        possibleGoals = m_ambientGoals;
        //        break;
        //    case GOAL_STATE.SUSPICIOUS:
        //        possibleGoals = m_suspiciousGoals;
        //        break;
        //    case GOAL_STATE.ALERT:
        //        possibleGoals = m_alertGoals;
        //        break;
        //}

        possibleGoals.AddRange(m_ambientGoals);
        possibleGoals.AddRange(m_suspiciousGoals);
        possibleGoals.AddRange(m_alertGoals);

        m_goalList.Clear();

        foreach (Goal goal in possibleGoals)
        {
            goal.DetermineGoalPriority(m_NPC);
            m_goalList.Add(goal);
        }
        IOrderedEnumerable<Goal> orderedList = m_goalList.OrderByDescending(goal => goal.m_goalPriority); //

        m_goalList = orderedList.ToList<Goal>();
    }

    IEnumerator ActionPlanning()
    {
        m_currentAction = null;

        while (m_currentAction == null && m_goalList.Count > 0)
        {
            Goal currentGoal = m_goalList[0];
            m_goalList.RemoveAt(0);
            m_currentAction = GetActionPlan(currentGoal);
        }

        yield return null;
    }

    //-----------------------------
    //GOAP STUFF
    //-----------------------------

    private AIAction GetActionPlan(Goal currentGoal)
    {
        ActionNode goalNode = new ActionNode(m_NPC, currentGoal);
        if (goalNode.m_invalidWorldStates.Count == 0) //In the case all world states are met already, then goal is already complete, get new goal?
            return null;

        List<ActionNode> openNodes = new List<ActionNode>();
        List<ActionNode> closedNodes = new List<ActionNode>();

        ActionNode currentNode = NewCurrentNode(goalNode); //Starting node

        NewOpenNodes(currentNode, openNodes, closedNodes); // open list setup

        if (goalNode.m_invalidWorldStates.Count == 0) //Goal required just one action to be satified, current action will do this
            return currentNode.m_AIAction;

        while (openNodes.Count > 0)
        {
            NewOpenNodes(currentNode, openNodes, closedNodes);

            if (openNodes.Count == 0)
                return null;

            currentNode = NewCurrentNode(GetLowestFScore(openNodes));

            if (goalNode.m_invalidWorldStates.Count == 0) //New current node just satisfied the goals final state
                return currentNode.m_AIAction;
        }

        return null;
    }

    private void NewOpenNodes(ActionNode currentNode, List<ActionNode> openNodes, List<ActionNode> closedNodes)
    {
        foreach (WorldState.WORLD_STATE worldState in currentNode.m_invalidWorldStates)
        {
            foreach (AIAction action in m_possibleActions)
            {
                if (action.m_satisfiedWorldStates.Contains(worldState))
                {
                    ActionNode newActionNode = NewActionNode(m_NPC, action, currentNode);
                    if (newActionNode != null)
                        openNodes.Add(newActionNode);
                }
            }
        }

        openNodes.Remove(currentNode);
        closedNodes.Add(currentNode);
    }

    private ActionNode NewCurrentNode(ActionNode newNode)
    {
        if (newNode.m_invalidWorldStates.Count == 0) //If this node is all valid, let parent know to update its validity
            newNode.m_previousNode.NewStateSatisfied(newNode);
        return newNode;
    }

    private ActionNode NewActionNode(NPC NPCScript, AIAction action, ActionNode previousNode)
    {
        ActionNode newNavNode = null;
        AIAction newAction = Instantiate(action);
        if (newAction.ActionInit(NPCScript, previousNode.m_AIAction)) //New action has been initiliased successfully
        {
            newNavNode = new ActionNode(NPCScript, newAction, previousNode);
        }
        return newNavNode;
    }

    private ActionNode GetLowestFScore(List<ActionNode> openNodes)
    {
        float fScore = Mathf.Infinity;
        ActionNode highestFNode = null;
        foreach (ActionNode node in openNodes)
        {
            if (node.m_fScore < fScore)
            {
                highestFNode = node;
                fScore = node.m_fScore;
            }
        }
        return highestFNode;
    }

    public class ActionNode
    {
        public List<ActionNode> m_nodes = new List<ActionNode>();
        public float m_fScore = 0.0f;
        private float m_gScore, m_hScore = 0.0f;
        public ActionNode m_previousNode = null;

        public AIAction m_AIAction = null;
        public List<WorldState.WORLD_STATE> m_invalidWorldStates = new List<WorldState.WORLD_STATE>();
        public List<ActionNode> m_satisfyingNodes = new List<ActionNode>();

        //--------------------------------------------------------------------------------------
        // Creation of a goal node, all this needs is to add a single invalid world state
        // 
        // Param
        //		NPCScript: Script to determine the agents world state
        //		goal: Goal to be satisfied
        //--------------------------------------------------------------------------------------
        public ActionNode(NPC NPCScript, Goal goal) //Designed for goal node
        {
            if (!WorldState.CheckForValidState(NPCScript, goal.m_desiredWorldState))
            {
                m_invalidWorldStates.Add(goal.m_desiredWorldState);
            }
            m_fScore = 0.0f;
        }

        //--------------------------------------------------------------------------------------
        // Creation of an action node, Gets all invalid states and determine F-Score for A*
        // 
        // Param
        //		NPCScript: Script to determine the agents world state
        //		action: Action to be added
        //		previousNode: parent node
        //--------------------------------------------------------------------------------------
        public ActionNode(NPC NPCScript, AIAction action, ActionNode previousNode)//Action Nodes
        {
            m_previousNode = previousNode;
            m_AIAction = action;

            GetInvalidStates(NPCScript);

            m_hScore = m_invalidWorldStates.Count();
            m_gScore = m_previousNode.m_gScore + m_AIAction.m_actionCost;

            m_fScore = m_hScore + m_gScore;
        }

        //--------------------------------------------------------------------------------------
        // Get all invalid states for a given agents world
        // 
        // Param
        //		NPCScript: Script to determine the agents world state
        //--------------------------------------------------------------------------------------
        private void GetInvalidStates(NPC NPCScript)
        {
            foreach (AIAction.RequiredWorldState worldState in m_AIAction.m_requiredWorldStates)
            {
                if (!WorldState.CheckForValidState(NPCScript, worldState.m_worldState))
                {
                    m_invalidWorldStates.Add(worldState.m_worldState);
                }
            }
        }

        //--------------------------------------------------------------------------------------
        //  On full satisfactoin of a nodes states, let parent know, to run back up the tree
        // This allows for knowledge of full tree validity
        //
        // Param
        //		satisfyingNode: Previous node which satisfies the world state, saved for later use
        //--------------------------------------------------------------------------------------
        public void NewStateSatisfied(ActionNode satisfyingNode)
        {
            foreach (WorldState.WORLD_STATE worldState in satisfyingNode.m_AIAction.m_satisfiedWorldStates)
            {
                m_invalidWorldStates.Remove(worldState);
            }

            m_satisfyingNodes.Add(satisfyingNode);

            if (m_invalidWorldStates.Count == 0 && m_previousNode != null)
                m_previousNode.NewStateSatisfied(this);
        }
    }

    //-----------------------------
    //END GOAP STUFF
    //-----------------------------
}
