using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class GameManager {

    [HideInInspector]
    public static Vector2 gridSize;
    
    //Global reference to all tiles in level
    public static GameObject[] tiles;
    public static GameObject[,] grid;
}
