using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ADD_REMOVE_FUNCTION { ADD, REMOVE }
public enum FACING_DIR { NORTH, EAST, SOUTH, WEST, NONE }
public enum INTERACTION_TYPE { NONE, WALL_HIDE, PICKUP_ITEM, INTERACTABLE, ATTACK, WALL_ATTACK, RANGED_ATTACK, REVIVE };

public class Global: MonoBehaviour
{
    public static int MAX_INT = 2147483647; //Largest int val
}

