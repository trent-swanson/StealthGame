using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDetection : StateMachineBehaviour
{
    public PlayerController m_playerController;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Mathf.Repeat(stateInfo.normalizedTime, 1.0f) >0.99f)
        {
            //m_playerController.EndAnimationState();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_playerController.EndAnimationState();
    }
}
