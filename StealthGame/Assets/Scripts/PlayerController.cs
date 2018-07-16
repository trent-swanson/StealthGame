using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Agent {

    void Awake() {
        Init();
    }

    void Start() {
    }

    void Update() {
        Debug.DrawRay(transform.position, transform.forward);

        //if not my turn then don't run Update()
        if (!turn)
            return;

        if (!moving && currentActionPoints > 0) {
            FindSelectableTiles();
            MouseClick();
        }
        else {
            Move(true);
        }
    }

    void MouseClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.tag == "Tile") {
                Tile t = hit.collider.GetComponent<Tile>();
                if (t.selectable) {
                    CheckMoveToTile(t, true);
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.tag == "Tile") {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if (t.selectable) {
                        CheckMoveToTile(t, false);
                    }
                }
            }
        }
    }
}
