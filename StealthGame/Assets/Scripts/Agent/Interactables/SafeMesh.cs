using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeMesh : MonoBehaviour
{
    private SoundController m_soundController = null;

    // Use this for initialization
    void Start ()
    {
        m_soundController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SoundController>();
    }

    public void PlaySafeOpening()
    {
        m_soundController.PlaySound(SoundController.SOUND.DOOR_SAFE);
    }

}
