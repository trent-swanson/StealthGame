using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal {

	public HashSet<KeyValuePair<string,object>> desiredWorldState = new HashSet<KeyValuePair<string, object>> ();
	public int priority;

	public Goal(string p_key, object p_value, int p_priority) {
		desiredWorldState.Add(new KeyValuePair<string, object>(p_key, p_value));
		priority = p_priority;
	}

	public int CalculateNewPriority() {
		return priority;
	}
}
