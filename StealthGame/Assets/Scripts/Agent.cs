using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float m_range = 5.0f;
    public float m_speed = 1.0f;

    protected TurnManager m_turnManager = null;

    protected virtual void Start()
    {
        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
    }

    public virtual void StartUnitTurn()
    {

    }

    public virtual void TurnUpdate()
    {

    }

    public void TurnEnd()
    {
        m_turnManager.EndUnitTurn();
    }


    public GameObject GetNodeBelow()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);

        Tile tile = hit.collider.GetComponent<Tile>();
        return tile.gameObject;
    }
}
