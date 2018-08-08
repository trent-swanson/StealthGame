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

    public enum CamDirection {NORTH, SOUTH, WEST, EAST}
    public static CamDirection camDirection;

    public static List<WallFade> walls = new List<WallFade>();

    public static void CheckCamDirection(Transform p_cam) {
        if (p_cam.rotation.y < 10 && p_cam.rotation.y > -10) {
            camDirection = CamDirection.SOUTH;
        }
        else if (p_cam.rotation.y < 100 && p_cam.rotation.y > 80) {
            camDirection = CamDirection.SOUTH;
        }
        else if (p_cam.rotation.y < -170 || p_cam.rotation.y > 170) {
            camDirection = CamDirection.SOUTH;
        }
        else {
            camDirection = CamDirection.SOUTH;
        }

        foreach (WallFade w in walls) {
            w.UpdateWallFade();
        }
    }
}
