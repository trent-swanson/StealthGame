using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrolling", menuName = "AI Goals/Patrolling")]
public class Patrolling : Goal {

	public override int DetermineGoalPriority(NPC NPCAgent) {
		int tempPriority = m_defualtPriority;
		return tempPriority;
	}
}
