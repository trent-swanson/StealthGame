using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour {

	public List<NPC> unitList = new List<NPC>();
	public List<AIAction> goals = new List<AIAction>();

	private List<NPC> processingUnit = new List<NPC>();

	enum PressureState { LOW, MEDIUM, HIGH, ULTRA }
	PressureState pressureState = PressureState.LOW;

	public void StartTurn() {
		AddUnitsToProcessing();
		AssignSquadGoals();
		AllUnitsPerformAction();
	}
	
	//CurrentWorldState
	//KnowladgeState

	//overrides agent goals
	void AssignSquadGoals() {
		switch (pressureState) {
			case PressureState.LOW:
			//check if multiple AI want to attack the same target

			//*** temp - remove this ***
			foreach (NPC agent in processingUnit) {
				agent.currentGoal = goals[0];
				agent.currentAction = agent.Plan(goals[0], null);
				Debug.Log(agent.currentAction.name);
			}
			break;
			case PressureState.MEDIUM:
			break;
			case PressureState.HIGH:
			break;
			case PressureState.ULTRA:
			break;
		}
	}

	void AllUnitsPerformAction() {
		foreach (NPC agent in processingUnit) {
			agent.currentAction.Perform(agent);
		}
	}

	void AddUnitsToProcessing() {
		foreach (NPC agent in unitList) {
			processingUnit.Add(agent);
			//agent.StartTurn();
		}
	}

	public void AgentHasFinishedActionPlan(NPC agent) {
		processingUnit.Remove(agent);
		if (processingUnit.Count <= 0) {
			TurnManager.EndAITurn();
		}
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
}
