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

    private PlayerMovementManager m_movementManager = null;

    private int m_navNodeLayer = 0;

    protected override void Start()
    {
        base.Start();
        m_camPivot = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();
        m_uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();

        m_movementManager = GetComponent<PlayerMovementManager>();

        m_navNodeLayer = LayerMask.GetMask("NavNode");
    }

    public override void StartUnitTurn() {
        m_camPivot.Focus(transform);
        m_uiController.UpdateUI(this);
        m_movementManager.PlayerSelected();
        BeginTurn();
    }

    public override void TurnUpdate() {
        if (!m_moving && m_currentActionPoints > 0) {
            m_movementManager.MovementManage();
        }
        else {
            Move(false);
        }
    }

    //void Update() {
    //    Debug.DrawRay(transform.position, transform.forward);
    //}

    //void MouseBehaviour() {
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_navNodeLayer)) {
    //        NavNode t = hit.collider.GetComponent<NavNode>();
    //        if (t.nodeState == NavNode.NodeState.SELECTABLE) {
    //            CalculatePathRender(CheckMoveToTile(t, true));
    //        } else if (t.nodeState != NavNode.NodeState.SELECTED) {
    //            ClearPathRender();
    //        }
    //    } else {
    //        ClearPathRender();
    //    }
    //    if (Input.GetMouseButtonUp(0)) {
    //        if (!EventSystem.current.IsPointerOverGameObject()) { //check if are not clicking on a UI element
    //            if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_navNodeLayer)) {
    //                NavNode t = hit.collider.GetComponent<NavNode>();
    //                if (t.nodeState == NavNode.NodeState.SELECTABLE) {
    //                    CheckMoveToTile(t, false);
    //                    ClearPathRender();
    //                    RemoveSelectableTiles();
    //                }
    //            }
    //        }
    //    }
    //}

    //void CalculatePathRender(Vector3[] p_path) {
    //    pathRenderer.positionCount = p_path.Length;
    //    pathRenderer.SetPositions(p_path);
    //}

    //public void ClearPathRender() {
    //    pathRenderer.positionCount = 0;
    //}
}
