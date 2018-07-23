using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateGrid : MonoBehaviour {

    public GameObject tilePrefab;

    public Vector2 gridSize;
    public float tileSize;
    [Range(0,1)]
    public float tilePadding;

    [ContextMenu("GenerateGrid")]

    private void Start() {
        GameManager.grid = new GameObject[(int)gridSize.x, (int)gridSize.y];
        //Generate();
    }

    public void Generate() {

        string holderName = "Generate Grid";
        //Destroy grid if already exists
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform gridHolder = new GameObject(holderName).transform;
        gridHolder.parent = transform;

        //instantiate new tiles, move them into a grid position, and add them to the GameManager.grid list
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                Vector3 tilePosition = new Vector3(-gridSize.x / 2 + (tileSize * x), -0.5f, -gridSize.y / 2 + (tileSize * y));
                GameObject newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                //newTile.transform.localScale = newTile.transform.localScale*(1 - tilePadding);
                newTile.transform.SetParent(gridHolder);
                GameManager.grid[x, y] = newTile;
            }
        }
    }
}
