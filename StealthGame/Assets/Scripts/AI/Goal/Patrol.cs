using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol", menuName = "AI Goals/Patrol")]
public class Patrol : Goal {

	public override int DetermineGoalPriority(Agent agent) {
		int tempPriority = m_defualtPriority;
		return tempPriority;
	}
}
