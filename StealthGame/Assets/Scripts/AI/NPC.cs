using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPC : Agent
{
    public enum State { AMBIENT, SUSPICIOUS, ALERT }
    public State m_state;

    //-----------------------
    // Agent States
    //-----------------------

    [System.Serializable]
    public class AgentWorldState {

        //Where agent is relative to node grid
        public Vector2Int m_currentNodePos= new Vector2Int(0, 0); 

        //Items I'm holding
        public List<Item> m_currentItems = new List<Item>();

        //Weapon information
        public enum WEAPON_TYPE { MELEE, RANGED }; //Fixed
        private WEAPON_TYPE m_weaponType = WEAPON_TYPE.MELEE; //Fixed

        //CurrentGoal
        public Goal m_goal = null;

        //Node this agent wants to go to
        public NavNode m_targetNode = null; //Fixed

        //Seen targets
        public List<Agent> m_possibleTargets = new List<Agent>(); //Realtime

        //Targets which have gone missing
        public List<InvestigationNode> m_investigationNodes = new List<InvestigationNode>(); //Realtime

        //Waypoints
        [SerializeField]
        public List<NavNode> m_waypoints = new List<NavNode>();
        public int m_waypointIndex = 0;
    }

    public struct InvestigationNode
    {
        private Agent m_target;
        public Agent Target
        {
            get { return m_target; }
            set { m_target = value; }
        }
        private Agent m_node;
        public Agent Node
        {
            get { return m_node; }
            set { m_node = value; }
        }
    }

    [Space]
    [Space]
    [Header("AgentState")]
    public AgentWorldState m_agentWorldState;

    [Space]
    public List<Goal> m_ambientGoals = new List<Goal>();
    public List<Goal> m_suspiciousGoals = new List<Goal>();
    public List<Goal> m_alertGoals = new List<Goal>();
    public List<AIAction> m_possibleActions = new List<AIAction>();

    [Space]
    public AIAction m_currentAction;

    [Space]
    public int m_currentActionNum = 0;
    public int m_maxActionNum = 3;

    [Space]
    private List<Agent> m_opposingTeam;


    protected override void Start()
    {
        base.Start();

        m_agentWorldState.m_goal = m_ambientGoals[0];

        m_opposingTeam = m_turnManager.GetOpposingTeam(m_team);

        //todo: need to remove this agent from the unitList when it is killed
        //squadManager.unitList.Add(this);
        m_state = State.AMBIENT;
    }

    private void Update()
    {
        foreach (Agent oppposingAgent in m_opposingTeam)
        {
            if(!m_agentWorldState.m_possibleTargets.Contains(oppposingAgent))
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, oppposingAgent.transform.position - transform.position, out hit))
                {
                    Agent hitAgent = hit.collider.GetComponent<Agent>();
                    if (hitAgent!=null)
                    {
                        m_agentWorldState.m_possibleTargets.Add(hitAgent);
                    }
                }
            }
        }    
    }

    public void DetermineGoal() {
        StartCoroutine("ActionPlanning");
    }

    public override void StartUnitTurn()
    {
        BeginTurn();
    }

    public override void TurnUpdate()
    {
        if (m_currentAction == null)
        {
            DetermineGoal();
            
            if(m_currentAction == null)//No valid action found
            {
                m_turnManager.EndUnitTurn(this);
                return;
            }

            m_currentAction.ActionStart(this);

            if (m_currentAction.m_actionCost > m_currentActionPoints)
            {
                m_turnManager.EndUnitTurn(this);
                return;
            }
        }

        m_currentAction.Perform(this);

        if (m_currentAction.IsDone(this))
        {
            m_currentAction.EndAction(this);
            m_currentActionPoints -= m_currentAction.m_actionCost;
            m_currentAction = null;
            EndAction();
            return;
        }
    }

    IEnumerator ActionPlanning() {
        //list of state goals
        //foreach goal DetermineGoalPriority() and add to a queue in order of cheapest
        //Find action plan for goal, if no plan found go to next goal
        //Tell Squad Manager what my goal is

        List<Goal> possibleGoals = new List<Goal>();

        switch (m_state)
        {
            case State.AMBIENT:
                possibleGoals = m_ambientGoals;
                break;
            case State.SUSPICIOUS:
                possibleGoals = m_suspiciousGoals;
                break;
            case State.ALERT:
                possibleGoals = m_alertGoals;
                break;
        }

        Queue<Goal> goalQueue = new Queue<Goal>();

        foreach (Goal goal in possibleGoals) {
            goal.DetermineGoalPriority(this);
            goalQueue.Enqueue(goal);
        }

        goalQueue.OrderByDescending(goal => goal.m_goalPriority);

        m_currentAction = null;

        while (m_currentAction == null && goalQueue.Count>0) {
            Goal currentGoal = goalQueue.Dequeue();

            m_currentAction = GetActionPlan(currentGoal);

            if (m_currentAction != null) {
                break;
            }
        }

        yield return null;
    }

    public Agent GetClosestTarget()
    {
        Agent possibleTarget = null;
        float targetDis = Mathf.Infinity;

        foreach (Agent agent in m_agentWorldState.m_possibleTargets) //TODO might have to update for multiple floors
        {
            if (agent == null)
                break;

            float sqrDis = Vector3.SqrMagnitude(agent.transform.position - transform.position);
            if(sqrDis< targetDis)
            {
                targetDis = sqrDis;
                possibleTarget = agent;
            }
        }
        return possibleTarget;
    }


    //-----------------------------
    //GOAP STUFF
    //-----------------------------

    private AIAction GetActionPlan(Goal currentGoal)
    {
        ActionNode goalNode = new ActionNode(this, currentGoal);
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

                    ActionNode newActionNode = NewActionNode(this, action, currentNode);
                    if(newActionNode!=null)
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
        if(newAction.ActionInit(NPCScript, previousNode.m_AIAction)) //New action has been initiliased successfully
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

            if (m_invalidWorldStates.Count == 0 && m_previousNode!=null)
                m_previousNode.NewStateSatisfied(this);
        }  
    }

    //-----------------------------
    //END GOAP STUFF
    //-----------------------------
}
