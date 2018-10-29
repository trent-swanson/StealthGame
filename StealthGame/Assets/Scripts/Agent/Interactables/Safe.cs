﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : Interactable
{
    public Item m_item = null;

    public int m_health = 3;

    private GameObject boozeGroup;
    private GameObject boozeEmpty;
    private GameObject sparkleParticle;

    protected override void Start()
    {
        base.Start();

        boozeGroup = gameObject.transform.Find("BoozeGroup").gameObject;
        sparkleParticle = gameObject.transform.Find("Sparkle_Multiple").gameObject;
        boozeEmpty = boozeGroup = gameObject.transform.Find("BoozeGroup_Empty").gameObject;
        boozeEmpty.SetActive(false);
    }

    public override bool CanPerform(Agent agent)
    {
        return m_usable && agent.m_agentInventory.AgentHasItem(m_requiredItem);
    }


    public override void PerformAction(Agent agent)
    {
        Item newItem = Instantiate(m_item);
        newItem.EquipItem(agent);
        GetComponentInChildren<Animator>().SetTrigger("Safe_Open");
        DisableInteractable();

        //After interaction, remove all points 
        agent.m_currentActionPoints = 0;

        HideObjects();
    }

    private void HideObjects()
    {
        boozeGroup.SetActive(false);
        sparkleParticle.SetActive(false);
        boozeEmpty.SetActive(true);
    }
}
