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

    static int pressure = 0;
    
    public static int Pressure {
        get { return pressure; }
        set { if(value <= 12 || value >= 0){
                pressure = value;
            } else if(value > 12) {
                pressure = 12;
            } else if(value < 0) {
                pressure = 0;
            }
        }
    }
}
