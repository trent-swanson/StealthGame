using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : Interactable
{
    public Item m_item = null;

    public int m_health = 3;

    private GameObject boozeGroup;
    private GameObject boozeEmpty;
    private GameObject sparkleParticle;

    //--------------------------------------------------------------------------------------
    // Initialisation
    // Ensure interactable has a conected canvas
    // Set navigation nodes to be a item based interatable 
    //
    // Set up visual ques 
    //--------------------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        boozeGroup = gameObject.transform.Find("BoozeGroup").gameObject;
        sparkleParticle = gameObject.transform.Find("Sparkle_Multiple").gameObject;
        boozeEmpty = boozeGroup = gameObject.transform.Find("BoozeGroup_Empty").gameObject;
        boozeEmpty.SetActive(false);
    }

    //--------------------------------------------------------------------------------------
    // Can agent use this interactable
    // Requires agent to have the required item
    // 
    // Param
    //		agent: agent to check condition against
    // Return:
    //      bool when usable
    //--------------------------------------------------------------------------------------
    public override bool CanPerform(Agent agent)
    {
        return m_usable && agent.m_agentInventory.AgentHasItem(m_requiredItem);
    }

    //--------------------------------------------------------------------------------------
    // Perform interactables actions
    // Play safe opening, give agent the item in safe
    //
    // Param
    //		agent: agent to perform action
    //--------------------------------------------------------------------------------------
    public override void PerformAction(Agent agent)
    {
        Item newItem = Instantiate(m_item);
        newItem.EquipItem(agent);
        GetComponentInChildren<Animator>().SetTrigger("Safe_Open");
        DisableInteractable();

        //After interaction, remove all points 
        agent.m_currentActionPoints = 0;

        PlayerController playerController = agent.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.AddGoldBag();

        base.PerformAction(agent);
        HideObjects();
    }

    //--------------------------------------------------------------------------------------
    // Remove attached objects to graphically show player taking items
    //--------------------------------------------------------------------------------------
    private void HideObjects()
    {
        boozeGroup.SetActive(false);
        sparkleParticle.SetActive(false);
        boozeEmpty.SetActive(true);
    }

    //--------------------------------------------------------------------------------------
    // Play sounds
    //--------------------------------------------------------------------------------------
    public void PlaySafeClick()
    {
        m_soundController.PlaySound(SoundController.SOUND.CLICK_MOVE_0);
    }
}
