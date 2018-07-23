using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unamed item", menuName = "Items/New Item")]
public class Item : ScriptableObject {

	public AIAction action;
	public Sprite icon;
	public string description;

	public void UseItem() {
		
	}

	public void PickUpItem() {

	}

	public void DropItem() {

	}
}
