using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class GenerateGrid : MonoBehaviour {

    public GameObject tilePrefab;

    public Vector2 gridSize;
    public float tileSize;
    [Range(0,1)]
    public float tilePadding;  

    string holderName = "Generate Grid";

    [ContextMenu("GenerateGrid")]

  //  void Awake() {
  //      GameManager.gridSize = gridSize;
  //      GameManager.tiles = new GameObject[(int)gridSize.x * (int)gridSize.y];
  //      GameManager.grid = new GameObject[(int)gridSize.x, (int)gridSize.y];
  //      Transform grid = transform.Find(holderName);
  //      for (int i = 0; i < grid.childCount; i++) {
  //          GameManager.tiles[i] = grid.GetChild(i).gameObject;
  //      }
  //      GetGrid();
  //  }

  //  void GetGrid() {
  //      int i = 0;
  //      for (int x = 0; x < gridSize.x; x++) {
		//	for (int y = 0; y < gridSize.y; y++) {
		//		GameManager.grid[x,y] = GameManager.tiles[i];
  //              i++;
		//	}
		//}
  //  }

    public void Generate() {
        //Destroy grid if already exists
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform gridHolder = new GameObject(holderName).transform;
        gridHolder.parent = transform;

        //instantiate new tiles, move them into a grid position, and add them to the GameManager.grid list
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                Vector3 tilePosition = new Vector3((-gridSize.x + (tileSize * 0.5f)) + (tileSize * x), -0.5f, (-gridSize.y + (tileSize * 0.5f)) + (tileSize * y));
                GameObject newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                //newTile.transform.localScale = newTile.transform.localScale*(1 - tilePadding);
                newTile.transform.SetParent(gridHolder);
            }
        }
    }
}
