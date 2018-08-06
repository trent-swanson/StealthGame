using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour {

	public List<NPC> unitList = new List<NPC>();
	public List<AIAction> goals = new List<AIAction>();

	private List<NPC> processingUnit = new List<NPC>();

	enum PressureState { LOW, MEDIUM, HIGH, ULTRA }
	PressureState pressureState = PressureState.LOW;

	
}
