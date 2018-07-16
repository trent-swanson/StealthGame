using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	static Dictionary<string, List<Agent>> units = new Dictionary<string, List<Agent>>();
    static Dictionary<string, List<EnvironmentObject>> environmentDictionary = new Dictionary<string, List<EnvironmentObject>>();
    static Queue<string> turnKey = new Queue<string>();
	static Queue<Agent> turnTeam = new Queue<Agent>();
    static Queue<EnvironmentObject> environmentTurn = new Queue<EnvironmentObject>();

    public delegate void UnitSelect(PlayerController p_unit);
	public static event UnitSelect OnUnitSelect;
	
	public delegate void UnitDeselect();
	public static event UnitDeselect OnUnitDeselect;

    /*
    * Important Note:
    * Check any code relating to agents being dead and removing them from turn manager
    * Agents can be knocked out, but then revived so must be readded
    */

	void Start() {
		//Find all tiles in level and add them to GameManager tile list
		GameManager.tiles = GameObject.FindGameObjectsWithTag("Tile");
	}

	void Update() {
		if (turnTeam.Count == 0) {
			InitTeamTurnMove();
		}
	}

    //initilise unit team
	static void InitTeamTurnMove() {
		List<Agent> teamList = units[turnKey.Peek()];

		foreach (Agent unit in teamList) {
			turnTeam.Enqueue(unit);
		}

		StartTurn();
	}

    //initilise environment team
    static void InitEnvironmentTurnMove() {
        List<EnvironmentObject> teamList = environmentDictionary[turnKey.Peek()];

        foreach (EnvironmentObject unit in teamList) {
            environmentTurn.Enqueue(unit);
        }

        StartTurn();
    }

    //start of unit turn
    public static void StartTurn() {
		if (turnTeam.Count > 0) {
			if (!turnTeam.Peek().knockedout) {
				if(OnUnitSelect != null && turnKey.Peek() == "Player") {
					OnUnitSelect(turnTeam.Peek().GetComponent<PlayerController>());
				}
				turnTeam.Peek().BeginTurn();
			}
			else
				RemoveUnit();
		}
	}

    //end of unit turn
	public static void EndTurn() {
		Agent unit = turnTeam.Dequeue();
		unit.EndTurn();

		if (turnTeam.Count > 0) {
			StartTurn();
		}
		else {
			if(OnUnitDeselect != null && turnKey.Peek() == "Player") {
				OnUnitDeselect();
			}
			string team = turnKey.Dequeue();
			turnKey.Enqueue(team);
			InitTeamTurnMove();
		}
	}

    //add agents
	public static void AddUnit(Agent p_unit) {
		List<Agent> list;

		if (!units.ContainsKey(p_unit.tag)) {
			list = new List<Agent>();
			units[p_unit.tag] = list;

			if (!turnKey.Contains(p_unit.tag)) {
				turnKey.Enqueue(p_unit.tag);
			}
		}
		else {
			list = units[p_unit.tag];
		}

		list.Add(p_unit);
	}

    //add environment objs
    public static void AddUnit(EnvironmentObject p_unit) {
        List<EnvironmentObject> list;

        if (!environmentDictionary.ContainsKey(p_unit.tag)) {
            list = new List<EnvironmentObject>();
            environmentDictionary[p_unit.tag] = list;

            if (!turnKey.Contains(p_unit.tag)) {
                turnKey.Enqueue(p_unit.tag);
            }
        }
        else {
            list = environmentDictionary[p_unit.tag];
        }

        list.Add(p_unit);
    }

    //remove agents
    public static void RemoveUnit() {
        //remove unit from turnTeam
        Agent tempUnit = turnTeam.Dequeue();

        //remove unity from dictionary
        List<Agent> list;
        list = units[tempUnit.tag];
        list.Remove(tempUnit);
        if (list.Count > 0) {
            units[tempUnit.tag] = list;
        }
        else {
            units.Remove(tempUnit.tag);
        }

        //remove gameobject
        Destroy(tempUnit.gameObject);

        //if still units in team start next turn, else initialise next team
        if (turnTeam.Count > 0) {
            StartTurn();
        }
        else {
            string team = turnKey.Dequeue();

            //if no unit type in dictionary, remove unit turnKey
            if (units.ContainsKey(tempUnit.tag)) {
                turnKey.Enqueue(team);
            }

            InitTeamTurnMove();
        }
    }

    //remove environment objects
    void RemoveEnvironment() {
        //remove unit from turnTeam
        EnvironmentObject tempUnit = environmentTurn.Dequeue();

        //remove unity from dictionary
        List<EnvironmentObject> list;
        list = environmentDictionary[tempUnit.tag];
        list.Remove(tempUnit);
        if (list.Count > 0) {
            environmentDictionary[tempUnit.tag] = list;
        }
        else {
            environmentDictionary.Remove(tempUnit.tag);
        }

        //remove gameobject
        Destroy(tempUnit.gameObject);

        //if still units in team start next turn, else initialise next team
        if (environmentTurn.Count > 0) {
            StartTurn();
        }
        else {
            string team = turnKey.Dequeue();

            //if no unit type in dictionary, remove unit turnKey
            if (environmentDictionary.ContainsKey(tempUnit.tag)) {
                turnKey.Enqueue(team);
            }

            InitEnvironmentTurnMove();
        }
    }
}
