using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public enum SOUND {CLICK_MOVE_0, CLICK_MOVE_1, CLICK_MOVE_2, CLICK_MOVE_3, UI_BUTTON_0, UI_BUTTON_1, WALK_0, WALK_1, WALK_2, WALK_3, WALK_4, SLIDE_0, SLIDE_1, SLIDE_2, SLIDE_3, DOOR_SAFE, DOOR_ENTRANCE_0, DOOR_ENTRANCE_1, PUNCH, MELEE_WEAPON, DEATH, COUNT};
    public AudioSource m_clickMove0 = null;
    public AudioSource m_clickMove1 = null;
    public AudioSource m_clickMove2 = null;
    public AudioSource m_clickMove3 = null;

    public AudioSource m_UIButton0 = null;
    public AudioSource m_UIButton1 = null;

    public AudioSource m_walk0 = null;
    public AudioSource m_walk1 = null;
    public AudioSource m_walk2 = null;
    public AudioSource m_walk3 = null;
    public AudioSource m_walk4 = null;

    public AudioSource m_slide0 = null;
    public AudioSource m_slide1 = null;
    public AudioSource m_slide2 = null;

    public AudioSource m_doorSafe = null;
    public AudioSource m_doorEntrance0 = null;
    public AudioSource m_doorEntrance1 = null;

    public AudioSource m_punch = null;
    public AudioSource m_meleeWeapon = null;
    public AudioSource m_death = null;

    private AudioSource[] m_soundArray;

    private void Start()
    {
        m_soundArray = new AudioSource[(int)SOUND.COUNT];

        m_soundArray[(int)SOUND.CLICK_MOVE_0] = m_clickMove0;
        m_soundArray[(int)SOUND.CLICK_MOVE_1] = m_clickMove1;
        m_soundArray[(int)SOUND.CLICK_MOVE_2] = m_clickMove2;
        m_soundArray[(int)SOUND.CLICK_MOVE_3] = m_clickMove3;
        m_soundArray[(int)SOUND.UI_BUTTON_0] = m_UIButton0;
        m_soundArray[(int)SOUND.UI_BUTTON_1] = m_UIButton1;
        m_soundArray[(int)SOUND.WALK_0] = m_walk0;
        m_soundArray[(int)SOUND.WALK_1] = m_walk1;
        m_soundArray[(int)SOUND.WALK_2] = m_walk2;
        m_soundArray[(int)SOUND.WALK_3] = m_walk3;
        m_soundArray[(int)SOUND.WALK_4] = m_walk4;
        m_soundArray[(int)SOUND.SLIDE_0] = m_slide0;
        m_soundArray[(int)SOUND.SLIDE_1] = m_slide1;
        m_soundArray[(int)SOUND.SLIDE_2] = m_slide2;
        m_soundArray[(int)SOUND.DOOR_SAFE] = m_doorSafe;
        m_soundArray[(int)SOUND.DOOR_ENTRANCE_0] = m_doorEntrance0;
        m_soundArray[(int)SOUND.DOOR_ENTRANCE_1] = m_doorEntrance1;
        m_soundArray[(int)SOUND.PUNCH] = m_punch;
        m_soundArray[(int)SOUND.MELEE_WEAPON] = m_meleeWeapon;
        m_soundArray[(int)SOUND.DEATH] = m_death;
    }

    public void PlaySound(SOUND sound)
    {
        if(m_soundArray[(int)sound]!=null)
            m_soundArray[(int)sound].Play();
    }
}
