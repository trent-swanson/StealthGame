using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Agent {

	[Space]
	[Space]
	[Header("Waypoints")]
	public List<GameObject> m_waypoints = new List<GameObject>();
	public int m_currentWaypoint = 0;

	[Space]
	[Space]
	public GameObject m_playerTarget;
	public GameObject m_workNodeTarget;
	public GameObject m_guardTarget;
	public GameObject m_target;

	public enum State { AMBIENT, SUSPICIOUS, ALERT }
	public State m_state;

	//-----------------------
    // Agent States
    //-----------------------

    [System.Serializable]
    public class AgentState {
        //Items I'm holding
        public List<Item> m_currentItems = new List<Item>();
        
        //Weapon information
        public enum WEAPON_TYPE {MELEE, RANGED }; //Fixed
        private WEAPON_TYPE m_weaponType = WEAPON_TYPE.MELEE; //Fixed


        //Node this agent wants to go to
        private GameObject m_targetNode = null; //Fixed

        //Seen targets
        private List<GameObject> m_possibleTargets = new List<GameObject>(); //Realtime

        //Targets which have gone missing
        private List<InvestigationNode> m_investigationNodes = new List<InvestigationNode>(); //Realtime

        //Waypoints
        [SerializeField]
        private List<GameObject> m_waypoints = new List<GameObject>();

        private int m_waypointIndex = 0;
    }

    public struct InvestigationNode
    {
        private GameObject m_target;
        public GameObject Target
        {
            get { return m_target; }
            set { m_target = value; }
        }
        private GameObject m_node;
        public GameObject Node
        {
            get { return m_node; }
            set { m_node = value; }
        }
    }

    [Space]
    [Space]
    [Header("AgentState")]
    public AgentState m_agentState;

    [Space]

    [SerializeField]
    public List<Goal> m_possibleGoals = new List<Goal>();
    public List<AIAction> m_possibleActions = new List<AIAction>();

	[Space]
	public AIAction m_currentAction;

	[Space]
	public int m_currentActionNum = 0;
	public int m_maxActionNum = 3;

	[Space]
    private List<GameObject> m_opposingTeam;


	void Start() {
		//todo: need to remove this agent from the unitList when it is killed
		GameObject.FindGameObjectWithTag("GameController").GetComponent<SquadManager>().unitList.Add(this);
		m_state = State.AMBIENT;

		//duck tape
        List<Agent> opposingAgents = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>().m_playerTeam;
        foreach (Agent agent in opposingAgents)
        {
            m_opposingTeam.Add(agent.gameObject);
        }

		Init();

		//temp - remove this - also duck tape
		m_state = State.AMBIENT;
	}

	public override void StartUnitTurn() {

		StartCoroutine("ActionPlanning");
		BeginTurn();

		//should be here?
        if (m_currentActionNum > m_maxActionNum)
            EndTurn();
	}

	public override void TurnUpdate() {
		if(m_currentAction != null)
			m_currentAction.Perform(this);
	}

	void Update() {
		Debug.DrawRay(transform.position, transform.forward);

		//Listen for inputs and change state
    }

	IEnumerator ActionPlanning() {
		//Determine Goal TODO, based off priority override, when one fails loop to next etc
        Goal currentGoal = m_possibleGoals[0];

        //Setup first action : what happens if no action found?
        m_currentAction = GetActionPlan(currentGoal);
		yield return null;
	}

	//Plan what sequence of actions can fulfill the goal
	//Returns null if a plan could not be found
	public AIAction GetActionPlan(Goal p_goal) {

		List<Node> openSet = new List<Node>();
		HashSet<Node> closeSet = new HashSet<Node>();

		//Node startNode = new Node();
		Node targetNode = new Node(p_goal);
		openSet.Add(targetNode);

		//A* ===================
		while (openSet.Count > 0) {
			Node currentNode = openSet[0];
			//get action Node with the smallest fcost (action cost + number of preconditions not yet met)
			if (openSet.Count > 1) {
				for (int i = 0; i < openSet.Count; i++) {
					if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
						currentNode = openSet[i];
					}
				}
			}

			openSet.Remove(currentNode);
			closeSet.Add(currentNode);

			//Found an action we can do now
			if (currentNode.hCost == 0) {
				Debug.Log("Got Action");
				return currentNode.action;
			}
			
			foreach (Node subNode in GetActionsThatMeetPreconditions(currentNode)) {
				if (!openSet.Contains(subNode)) {
					openSet.Add(subNode);
				}
			}
		}

		//failed to find a plan for this goal
		Debug.Log("Plan Failed");
		return null;
	}

	//these 2 functions get a list of all the sub nodes that meet the preconditions of the current node
	private List<Node> GetActionsThatMeetPreconditions(Node p_node) {
		List<Node> subActionList = new List<Node>();

		// go through each action available and see if we can use it here
		foreach (AIAction action in m_possibleActions) {

			//check if any of current nodes m_requiredWorldStates are met by action's m_satisfiedWorldStates
			if (CheckPreconditionsAreMet(p_node.action, action)) {
				subActionList.Add(new Node(this, action, p_node));
			}
		}
		return subActionList;
	}

	private bool CheckPreconditionsAreMet(AIAction p_currentAction, AIAction p_action) {
		foreach (WorldState.WORLD_STATE reqiredState in p_currentAction.m_requiredWorldStates) {
			if (p_action.m_satisfiedWorldStates.Contains(reqiredState)) {
				return true;
			}
		}
		return false;
	}


	//Apply the stateChange to the currentState
	private HashSet<KeyValuePair<string,object>> PopulateWorldState(HashSet<KeyValuePair<string,object>> p_currentState, HashSet<KeyValuePair<string,object>> p_stateChange) {
		HashSet<KeyValuePair<string,object>> state = new HashSet<KeyValuePair<string,object>> ();
		// copy the KVPs over as new objects
		foreach (KeyValuePair<string,object> s in p_currentState) {
			state.Add(new KeyValuePair<string, object>(s.Key,s.Value));
		}

		foreach (KeyValuePair<string,object> change in p_stateChange) {
			// if the key exists in the current state, update the Value
			bool exists = false;

			foreach (KeyValuePair<string,object> s in state) {
				if (s.Equals(change)) {
					exists = true;
					break;
				}
			}

			if (exists) {
				state.RemoveWhere( (KeyValuePair<string,object> kvp) => { return kvp.Key.Equals (change.Key); } );
				KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key,change.Value);
				state.Add(updated);
			}
			// if it does not exist in the current state, add it
			else {
				state.Add(new KeyValuePair<string, object>(change.Key,change.Value));
			}
		}
		return state;
	}

	private class Node {
		public Node parent;
		public int gCost;
		public int hCost;
		public int fCost { get { return gCost + hCost; } }
		public AIAction action;
		public Goal goal;

		public Node (Agent p_agent, AIAction p_action) {
			if (p_action != null) {
				gCost = p_action.m_actionCost;
				hCost = DetermineHCost(p_agent, p_action.m_requiredWorldStates);
				action = p_action;
			}
			parent = null;
		}

		public Node (Agent p_agent, AIAction p_action, Node p_parent) {
			if (p_action != null) {
				gCost = p_action.m_actionCost;
				hCost = DetermineHCost(p_agent, p_action.m_requiredWorldStates);
				action = p_action;
			}
			parent = p_parent;
		}

		public Node (Goal p_goal) {
			gCost = p_goal.m_goalPriority;
			hCost = 1;
			action = null;
			parent = null;
		}

		private int DetermineHCost(Agent p_agent, List<WorldState.WORLD_STATE> p_requiredWorldStates) {
			int tempHCost = 0;
			foreach (WorldState.WORLD_STATE wState in p_requiredWorldStates) {
				if (!WorldState.CheckForValidState(p_agent, wState)) {
					tempHCost++;
				}
			}
			return tempHCost;
		}

	}
}
