using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTutorial : MonoBehaviour
{
    public PlayerController m_player = null;
    private AgentInventory m_playerInventory = null;

    public GameObject m_crowbarParticle = null;
    public Item m_crowbarItem = null;

    public GameObject m_safeParticle = null;
    public GameObject m_safeObject = null;
    private Safe m_safeScript = null;

    public GameObject m_doorParticle = null;

    public enum TUTORIAL_STATE {GET_CROWBAR, OPEN_SAFE, LEAVE_DOOR }
    public TUTORIAL_STATE m_currentState = TUTORIAL_STATE.GET_CROWBAR;
    private void Start()
    {
        m_crowbarParticle.GetComponent<ParticleSystem>().Play();
        m_safeParticle.GetComponent<ParticleSystem>().Stop();
        m_doorParticle.GetComponent<ParticleSystem>().Stop();

        m_playerInventory = m_player.m_agentInventory;
        m_safeScript = m_safeObject.GetComponent<Safe>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (m_playerInventory.AgentHasItem(m_crowbarItem) && m_currentState == TUTORIAL_STATE.GET_CROWBAR)
        {
            ParticleSystem.MainModule main = m_crowbarParticle.GetComponent<ParticleSystem>().main;
            main.loop = false;

            m_safeParticle.GetComponent<ParticleSystem>().Play();

            m_currentState = TUTORIAL_STATE.OPEN_SAFE;
        }

        if(!m_safeScript.m_usable && m_currentState == TUTORIAL_STATE.OPEN_SAFE)
        {
            ParticleSystem.MainModule main = m_safeParticle.GetComponent<ParticleSystem>().main;
            main.loop = false;

            m_doorParticle.GetComponent<ParticleSystem>().Play();

            m_currentState = TUTORIAL_STATE.LEAVE_DOOR;
        }
	}
}
