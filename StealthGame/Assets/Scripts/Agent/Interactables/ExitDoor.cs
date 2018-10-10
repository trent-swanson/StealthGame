using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Interactable
{
    public Item.ITEM_TYPE m_requiredItem = Item.ITEM_TYPE.GOLD;

    protected override void Start()
    {
        base.Start();
    }

    public override void PerformAction(Agent agent)
    {
        if (agent.m_agentInventory.AgentHasItem(m_requiredItem))
        {
            m_gameController.GetComponent<GameState_TurnManager>().m_objectiveAchived = true;
        }
    }
}
