using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour {

	public Item item;
	public GameObject canvas;
	bool isInRange = false;
	bool beingPickedUp = false;
	Tile occupingTile;

	public Color highlightColour;
    public float outlineWidth = 1.21f;

	void Start() {
		canvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = item.icon;
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 5)) {
			if (hit.transform.tag == "Tile") {
				occupingTile = hit.transform.GetComponent<Tile>();
			}
		}
	}

	void OnMouseEnter() {
		if (isInRange) {
			canvas.SetActive(true);
		}
	}

	void OnMouseOver() {
		if (isInRange && Input.GetMouseButtonDown(0)) {
			if (occupingTile.selectable) {
				occupingTile.selectableBy.CheckMoveToTile(occupingTile, false);
				beingPickedUp = true;
			}
		}
	}

	void OnMouseExit() {
		canvas.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
		if (beingPickedUp && other.tag == "Player" || beingPickedUp && other.tag == "NPC") {
			occupingTile.selectableBy.AddItem(item);
			Destroy(this.gameObject);
		}
	}

	public void PickedUp(Agent p_agent) {
		p_agent.m_currentItems.Add(item);
		Destroy(this);
	}

	public void TurnOnOutline() {
		Shader outlineShader = Shader.Find("Custom/Outline");
		if (outlineShader != null) {
			Renderer renderer = GetComponent<Renderer>();
			renderer.material.shader = outlineShader;
			renderer.material.SetColor("_OutlineColor", highlightColour);
            renderer.material.SetFloat("_OutlineWidth", outlineWidth);
        }
		isInRange = true;
	}

	public void TurnOffOutline() {
		Shader standardShader = Shader.Find("Standard");
		if (standardShader != null) {
			Renderer renderer = GetComponent<Renderer>();
			renderer.material.shader = standardShader;
		}
		isInRange = false;
	}
}
