using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Agent {

    [Space]
    [Space]
    public Sprite portrait;

    static CameraController m_camPivot;
    static UIController m_uiController;

    protected override void Start()
    {
        base.Start();
        m_camPivot = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();
    }

    public override void StartUnitTurn() {
        m_camPivot.Focus(transform);
        m_uiController.UpdateUI(this);
        BeginTurn();
    }

    public override void TurnUpdate() {
        if (!m_moving && m_currentActionPoints > 0) {
            FindSelectableTiles();
            MouseClick();
        }
        else {
            Move(true);
        }
    }

    void Update() {
        Debug.DrawRay(transform.position, transform.forward);
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
            if (!EventSystem.current.IsPointerOverGameObject()) { //check if are not clicking on a UI element
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
}
