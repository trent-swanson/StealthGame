using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDetection : StateMachineBehaviour
{
    public PlayerController m_playerController;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //m_playerController.EndAnimationState();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_playerController.EndAnimationState();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //if(animatorStateInfo.length < animatorStateInfo.normalizedTime)
           // m_playerController.EndAnimationState();
    }
}
