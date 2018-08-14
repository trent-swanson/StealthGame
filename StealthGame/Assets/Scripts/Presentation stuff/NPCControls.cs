using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCControls : Agent
{
    public enum ANIMATION {NONE, RUN_TO_IDLE, IDLE_TO_RUN, DEATH }

    Dictionary<ANIMATION, string> m_animtionToString = new Dictionary<ANIMATION, string>() { { ANIMATION.NONE, "" }, { ANIMATION.RUN_TO_IDLE, "RunToIdle" }, { ANIMATION.IDLE_TO_RUN, "IdleToRun" }, { ANIMATION.DEATH, "Death" } };

    [System.Serializable]
    public struct moveorder
    {
        public NavNode navNode;
        public ANIMATION animationTrigger;
        public bool endTurn;
    }

    public List<moveorder> m_moveOrders = new List<moveorder>();


    int m_moveOrdersIndex = 0;
    protected override void Start()
    {
        base.Start();
    }


    //Start of turn, only runs once per turn
    public override void AgentTurnInit()
    {
        base.AgentTurnInit();
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public override void AgentSelected()
    {
        m_animator.SetTrigger(m_animtionToString[m_moveOrders[m_moveOrdersIndex].animationTrigger]);
        FaceDir(m_moveOrders[m_moveOrdersIndex].navNode);
    }

    //Constant update while agent is selected
    public override void AgentTurnUpdate()
    {
        if(m_moveOrders[m_moveOrdersIndex].navNode != null)
        {
            if (MoveTo(m_moveOrders[m_moveOrdersIndex].navNode))
            {
                if (m_moveOrders[m_moveOrdersIndex].endTurn)
                {
                    m_currentActionPoints = 0;
                    m_turnManager.EndUnitTurn(this);
                }

                m_moveOrdersIndex++;
                if (m_moveOrdersIndex >= m_moveOrders.Count)
                    m_moveOrdersIndex = 0;

                if(m_currentActionPoints>0) //hack
                {

                    if (m_moveOrders[m_moveOrdersIndex].navNode != null)
                        FaceDir(m_moveOrders[m_moveOrdersIndex].navNode);

                    if (m_animtionToString[m_moveOrders[m_moveOrdersIndex].animationTrigger] != "")
                        m_animator.SetTrigger(m_animtionToString[m_moveOrders[m_moveOrdersIndex].animationTrigger]);
                }
            }
        }
}

    //Runs when agent is removed from team list, end of turn
    public override void AgentTurnEnd()
    {
        base.AgentTurnEnd();
    }


    private void FaceDir(NavNode pathNode)
    {
        Vector3 velocityVector = pathNode.m_nodeTop - transform.position;
        Vector3 dir = velocityVector.normalized;
        dir.y = 0;
        transform.LookAt(transform.position + dir);
    }

    private bool MoveTo(NavNode pathNode)
    {
        Vector3 targetPos = pathNode.m_nodeTop;
        Vector3 velocityVector = targetPos - transform.position;
        velocityVector.y = 0;
        float translateDis = velocityVector.magnitude;

        velocityVector = velocityVector.normalized * Time.deltaTime * m_moveSpeed;

        if (velocityVector.magnitude > translateDis)//Arrived at node
        {
            transform.position = targetPos;
            m_currentNavNode = pathNode;
            return true;
        }
        else
        {
            transform.position += velocityVector;
        }
        return false;
    }
}
