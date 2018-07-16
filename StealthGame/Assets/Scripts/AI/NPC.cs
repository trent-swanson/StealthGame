using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Agent {

	[Space]
	[Space]
	public GameObject playerTarget;
	public GameObject workNodeTarget;
	public GameObject guardTarget;
	public GameObject target;

	private enum State { AMBIENT, SUSPICIOUS, ALERT }
	private State state;

	[Space]
	[Space]
	[Header("Waypoints")]
	public List<GameObject> waypoints = new List<GameObject>();
	public int currentWaypoint = 0;

	public class Goal {
		bool desiredWorldState = false;
	}


	void Start() {
		//todo: need to remove this agent from the unitList when it is killed
		GameObject.FindGameObjectWithTag("GameController").GetComponent<SquadManager>().unitList.Add(this);
		state = State.AMBIENT;
		Init();
	}

	void Update() {
		Debug.DrawRay(transform.position, transform.forward);

        //if not my turn then don't run Update()
        if (!turn)
            return;

        if (!moving && currentActionPoints > 0) {
			//FindNearestTarget();
			NextWaypoint();
			CalculatePath();
            //FindSelectableTiles();
			actualTargetTile.target = true;
        }
        else {
            Move(false);
        }
    }

	void CalculatePath() {
		Tile targetTile = GetTargetTile(target);
		if (waypoints.Count > 0)
			FindPath(targetTile, true);
		else
			FindPath(targetTile, false);
	}

	void NextWaypoint() {
		if (waypoints.Count == 0) {
			//FindNearestTarget();
			return;
		}
		if (Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) < 0.15f) {
			currentWaypoint++;
			if (currentWaypoint >= waypoints.Count) {
				currentWaypoint = 0;
			}
		}
		target = waypoints[currentWaypoint];
	}

	/*
	void FindNearestTarget() {
		//find all players, change this to accept waypoints
		GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

		GameObject nearest = null;
		float distance = Mathf.Infinity;

		//find closes target in targets array
		foreach (GameObject obj in targets) {
			//SqrMagnitude is more efficent than vector3.Distance
			float dis = Vector3.SqrMagnitude(transform.position - obj.transform.position);
			if (dis < distance) {
				distance = dis;
				nearest = obj;
			}
		}

		target = nearest;
	}*/

	//========================================================================================	
	public List<Action> availableActions = new List<Action>();
	private Stack<Action> actionPlan = new Stack<Action>();
	private HashSet<KeyValuePair<string,object>> currentWorldState;
	public Goal currentGoal;

	//todo - IsValid chack on goals (e.g. does the AI know where the player is, if not dont do Kill Enemy Goal)

	//Plan what sequence of actions can fulfill the goal
	//Returns null if a plan could not be found
	public Action Plan(GameObject p_agent, HashSet<KeyValuePair<string,object>> desiredWorldState) {
		// reset the actions so we can start fresh with them
		foreach (Action a in availableActions) {
			a.DoReset ();
		}

		// check what actions can run using their checkProceduralPrecondition
		List<Action> usableActions = new List<Action> ();
		foreach (Action a in availableActions) {
			if ( a.CheckProceduralPrecondition(p_agent, target) )
				usableActions.Add(a);
		}

		List<Node> openSet = new List<Node>();
		HashSet<Node> closeSet = new HashSet<Node>();

		Node startNode = new Node(null, null, currentWorldState);
		Node targetNode = new Node(null, null, desiredWorldState);
		openSet.Add(targetNode);

		//==================
		//A* ===================
		while (openSet.Count > 0) {
			Node currentNode = openSet[0];
			//get action Node with the smallest cost (action cost + number of preconditions)
			if (openSet.Count > 1) {
				for (int i = 0; i < openSet.Count; i++) {
					if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
						currentNode = openSet[i];
					}
				}
			}

			openSet.Remove(currentNode);
			closeSet.Add(currentNode);

			if (currentNode.worldState == startNode.worldState) {
				return currentNode.action;
			}
			
			foreach (Node subNode in GetActionsThatMeetPreconditions(currentNode, usableActions)) {
				if (!openSet.Contains(subNode)) {
					openSet.Add(subNode);
				}
			}
		}

		//failed to find a plan for this goal
		return null;
	}

	//these 2 functions get a list of all the sub nodes that meet the preconditions of the current node
	private List<Node> GetActionsThatMeetPreconditions(Node p_node, List<Action> p_usableActions) {
		List<Node> subActionList = new List<Node>();

		// go through each action available and see if we can use it here
		foreach (Action action in p_usableActions) {

			//check if any of p_node's preconditions are met by action effects
			if (CheckPreconditionsAreMet(p_node.action.Preconditions, action.Effects)) {
				subActionList.Add(new Node(action, p_node, action.Preconditions));
			}
		}
		return subActionList;
	}
	private bool CheckPreconditionsAreMet(HashSet<KeyValuePair<string,object>> p_preconditions, HashSet<KeyValuePair<string,object>> p_effects) {
		bool match = false;
		foreach (KeyValuePair<string,object> p in p_preconditions) {
			foreach (KeyValuePair<string,object> e in p_effects) {
				if (e.Equals(p)) {
					match = true;
					break;
				}
			}
		}
		return match;
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
		public HashSet<KeyValuePair<string, object>> worldState;
		public Action action;

		public Node (Action p_action, Node p_parent, HashSet<KeyValuePair<string, object>> p_worldState) {
			parent = p_parent;
			gCost = p_action.cost;
			hCost = p_action.Preconditions.Count;
			action = p_action;
			worldState = p_worldState;
		}
	}
}
