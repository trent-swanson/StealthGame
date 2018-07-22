using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol", menuName = "AIBehaviours/Goals/Patrol")]
public class Patrol : Goal
{
    public override float DetermineGoalPriority(GameObject agent)
    {
        return 0.0f;
    }
}
