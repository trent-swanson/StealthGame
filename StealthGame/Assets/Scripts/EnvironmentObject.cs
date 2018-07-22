using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour {

    public bool active = false;

    Rigidbody rb;

    public Vector3 velocity;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        //TurnManager.AddUnit(this);
    }

    void Update() {

    }

    public void StartTurn() {

    }

    public void EndTurn() {

    }
}
