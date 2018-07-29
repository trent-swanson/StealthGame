using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTileOutline : MonoBehaviour {

	Mesh mesh;
	Vector3[] vertices;
	int[] gridTriangles;
	int[] triangles;
	Vector2[] uv;
	Vector4[] tangents;
	Vector4 tangent;

	//grid settings
	public float cellSize = 1;
	public Vector3 gridOffset;
	Vector2 gridSize;

	void Awake() {
		mesh = GetComponent<MeshFilter>().mesh;
	}

	public void GetSelectableTileOutline() {
		gridSize = GameManager.gridSize;
		gridOffset = new Vector3(-GameManager.gridSize.x + (cellSize * 0.5f), 0, -GameManager.gridSize.y + (cellSize * 0.5f));
		MakeContigousProceduralGrid();
		CutOutUnselectedTiles();
		UpdateMesh();
	}

	void MakeContigousProceduralGrid() {
		//set array sizes
		vertices = new Vector3[((int)gridSize.x + 1) * ((int)gridSize.y + 1)];
		gridTriangles = new int[(int)gridSize.x * (int)gridSize.y * 6];
		uv = new Vector2[vertices.Length];
		tangents = new Vector4[vertices.Length];
		tangent = new Vector4(1f, 0f, 0f, -1f);

		//set tracker ints
		int v = 0;
		int t = 0;

		//set vertex offset
		float vertexOffset = cellSize * 0.5f;

		//create vertex grid
		for (int x = 0; x <= gridSize.x; x++) {
			for (int y = 0; y <= gridSize.y; y++) {
				vertices[v] = new Vector3((x * cellSize) - vertexOffset, 0, (y * cellSize) - vertexOffset) + gridOffset;
				uv[v] = new Vector2((float)x / gridSize.x, (float)y / gridSize.y);
				tangents[v] = tangent;
				v++;
			}
		}

		//reset vertext tracker
		v = 0;

		//setting each cell's triangles
		for (int x = 0; x < gridSize.x; x++) {
			for (int y = 0; y < gridSize.y; y++) {
				gridTriangles[t] = v;
				gridTriangles[t+1] = gridTriangles[t+4] = v + 1;
				gridTriangles[t+2] = gridTriangles[t+3] = v + ((int)gridSize.y + 1);
				gridTriangles[t+5] = v + ((int)gridSize.y + 1) + 1;
				v++;
				t += 6;
			}
			v++;
		}
	}

	void CutOutUnselectedTiles() {
		int trisToDelete = 0;
		int i = 0;
		int j = 0;

		foreach (GameObject t in GameManager.tiles) {
			Tile tile = t.GetComponent<Tile>();
			if (!tile.selectable) {
				trisToDelete += 6;
			}
		}

		triangles = new int[gridTriangles.Length - trisToDelete];

		for (int x = 0; x < gridSize.x; x++) {
			for (int y = 0; y < gridSize.y; y++) {
				Tile tile = GameManager.grid[x,y].GetComponent<Tile>();
				if (tile.selectable) {
					triangles[i++] = gridTriangles[j++];
					triangles[i++] = gridTriangles[j++];
					triangles[i++] = gridTriangles[j++];
					triangles[i++] = gridTriangles[j++];
					triangles[i++] = gridTriangles[j++];
					triangles[i++] = gridTriangles[j++];
				} else {
					j += 6;
				}
			}
		}
	}

	void UpdateMesh() {
		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.tangents = tangents;
		mesh.RecalculateNormals();
	}


	//not in use
	/*
	void MakeDiscreteProceduralGrid() {
		//set array sizes
		vertices = new Vector3[(int)gridSize.x * (int)gridSize.y * 4];
		triangles = new int[(int)gridSize.x * (int)gridSize.y * 6];

		//set tracker ints
		int v = 0;
		int t = 0;

		//set vertex offset
		float vertexOffset = cellSize * 0.5f;

		for (int x = 0; x < gridSize.x; x++) {
			for (int y = 0; y < gridSize.y; y++) {
				Vector3 cellOffset = new Vector3(x * cellSize, 0, y * cellSize);

				//populate the vertices and triangles arrays
				vertices[v] = new Vector3(-vertexOffset, 0, -vertexOffset) + cellOffset + gridOffset;
				vertices[v+1] = new Vector3(-vertexOffset, 0,  vertexOffset) + cellOffset + gridOffset;
				vertices[v+2] = new Vector3( vertexOffset, 0, -vertexOffset) + cellOffset + gridOffset;
				vertices[v+3] = new Vector3( vertexOffset, 0,  vertexOffset) + cellOffset + gridOffset;

				triangles[t] = v;
				triangles[t+1] = triangles[t+4] = v+1;
				triangles[t+2] = triangles[t+3] = v+2;
				triangles[t+5] = v+3;

				v += 4;
				t += 6;
			}
		}
	}*/
}
