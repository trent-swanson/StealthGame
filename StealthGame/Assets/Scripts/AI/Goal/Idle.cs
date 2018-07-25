using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "AI Goals/Idle")]
public class Idle : Goal {

	public override int DetermineGoalPriority(Agent agent) {
		int tempPriority = m_defualtPriority;
		return tempPriority;
	}
}