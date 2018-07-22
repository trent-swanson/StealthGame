using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAgent : Agent
{
    [SerializeField]
    private List<Goal> m_possibleGoals = new List<Goal>();
    public List<Goal> PossibleGoals
    {
        get { return m_possibleGoals; }
    }

    [SerializeField]
    private List<Action> m_possibleActions = new List<Action>();

    public List<Action> PossibleActions
    {
        get { return m_possibleActions; }
    }

    NPCAgentPlanner m_NPCAgentPlanner = null;

    protected override void Start()
    {
        base.Start();
        m_NPCAgentPlanner = gameObject.GetComponent<NPCAgentPlanner>();
    }

    public override void StartUnitTurn()
    {
        m_NPCAgentPlanner.StartUnitTurn();
    }

    public override void TurnUpdate()
    {
        m_NPCAgentPlanner.TurnUpdate();
    }
}
