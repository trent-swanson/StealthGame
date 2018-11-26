using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeMesh : MonoBehaviour
{
    private SoundController m_soundController = null;

    //--------------------------------------------------------------------------------------
    // Initialisation
    //--------------------------------------------------------------------------------------
    void Start ()
    {
        m_soundController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SoundController>();
    }

    //--------------------------------------------------------------------------------------
    // Play safe opening sound
    //--------------------------------------------------------------------------------------
    public void PlaySafeOpening()
    {
        m_soundController.PlaySound(SoundController.SOUND.DOOR_SAFE);
    }

}
