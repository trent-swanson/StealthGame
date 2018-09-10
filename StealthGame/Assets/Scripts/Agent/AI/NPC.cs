﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Agent
{
    public GOAP m_GOAP = null;
    public AgentAnimationController m_agentAnimationController = null;

    //-----------------------
    // Agent States
    //-----------------------
    [System.Serializable]
    public class AgentWorldState
    {
        public bool m_modifiedFlag = false;

        //Weapon information
        public enum WEAPON_TYPE { MELEE, RANGED }; //Fixed

        [SerializeField]
        private WEAPON_TYPE m_weaponType = WEAPON_TYPE.MELEE; //Fixed
        public void SetWeapon(WEAPON_TYPE weapon) { m_weaponType = weapon; m_modifiedFlag = true; }
        public WEAPON_TYPE GetWeapon() { return m_weaponType; }

        //Seen targets
        [SerializeField]
        private List<Agent> m_possibleTargets = new List<Agent>(); //Realtime
        public void SetPossibleTargets(List<Agent> possibleTargets) { m_possibleTargets = possibleTargets; m_modifiedFlag = true; }
        public List<Agent> GetPossibleTargets() { return m_possibleTargets; }

        //Targets which have gone missing
        [SerializeField]
        private List<InvestigationNode> m_investigationNodes = new List<InvestigationNode>(); //Realtime
        public void SetInvestigationNode(List<InvestigationNode> investigationNodes) { m_investigationNodes = investigationNodes; m_modifiedFlag = true; }
        public List<InvestigationNode> GetInvestigationNodes() { return m_investigationNodes; }

        //Waypoints
        [SerializeField]
        public List<NavNode> m_waypoints = new List<NavNode>();
        public int m_waypointIndex = 0;
    }

    public struct InvestigationNode
    {
        private Agent m_target;
        public Agent Target
        {
            get { return m_target; }
            set { m_target = value; }
        }
        private Agent m_node;
        public Agent Node
        {
            get { return m_node; }
            set { m_node = value; }
        }
    }

    //Node this agent wants to go to
    public NavNode m_targetNode = null; //Fixed

    public List<NavNode> m_visionNodes = new List<NavNode>();

    [Space]
    [Space]
    [Header("AgentState")]
    public AgentWorldState m_agentWorldState;

    [Space]
    private List<Agent> m_opposingTeam;

    protected override void Start()
    {
        base.Start();

        m_agentAnimationController = GetComponent<AgentAnimationController>();
        m_GOAP = GetComponent<GOAP>();
        m_opposingTeam = m_turnManager.GetOpposingTeam(m_team);
    }


    //Start of turn, only runs once per turn
    public override void AgentTurnInit()
    {
        base.AgentTurnInit();
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public override void AgentSelected()
    {
        
    }

    //Constant update while agent is selected
    public override void AgentTurnUpdate()
    {
        //Check for update in world state
        if(m_agentWorldState.m_modifiedFlag)
        {
            Debug.Log("FlagMod");
            m_agentWorldState.m_modifiedFlag = false;
        }


        if(m_GOAP.m_actionList.Count == 0)//Checking if at the end of the action list
        {
            bool newAction = m_GOAP.GOAPInit();

            if(!newAction)//Unable to get a new action
            {
                m_currentActionPoints = 0;
                AgentTurnEnd();
                m_turnManager.EndUnitTurn(this);
                return;
            }
        }

        GOAP.GOAP_UPDATE_STATE actionState = m_GOAP.GOAPUpdate();

        switch (actionState)
        {
            case GOAP.GOAP_UPDATE_STATE.INVALID://Remove one as it attempted to occur
                m_currentActionPoints -= 1;
                m_GOAP.m_actionList.Clear();
                break;
            case GOAP.GOAP_UPDATE_STATE.COMPLETED:
                m_currentActionPoints -= m_GOAP.m_actionList[0].m_actionCost;
                m_GOAP.m_actionList.RemoveAt(0);
                break;
            case GOAP.GOAP_UPDATE_STATE.PERFORMING:
            default:
                break;
        }
    }

    //Runs when agent is removed from team list, end of turn
    public override void AgentTurnEnd()
    {
        BuildVision();
        base.AgentTurnEnd();
    }

    //Update the NPCs world stae, this will be called after every animation played, NPC or Player.
    public void UpdateWorldState()
    {
        BuildVision();//Build vision

        foreach (NavNode navNode in m_visionNodes)//Check for player in vision
        {
            Agent obstructingAgent = navNode.m_obstructingAgent;
            if (obstructingAgent != null && obstructingAgent.m_team != m_team)//Vision node has enemy agent on it
            {
                List<Agent> possibleTargets = m_agentWorldState.GetPossibleTargets();
                if (!possibleTargets.Contains(obstructingAgent))
                {
                    possibleTargets.Add(obstructingAgent);
                    m_agentWorldState.SetPossibleTargets(possibleTargets);
                }
            }
        }
    }

    private void BuildVision()
    {
        foreach (NavNode navNode in m_visionNodes) //Remove old vision
        {
            navNode.NPCVision(this, NavNode.ADD_REMOVE_FUNCTION.REMOVE);
        }

        m_visionNodes.Clear();

        List<NavNode> visibleNavNode = Vision.BuildVisionList(this);

        //Build vision cone, dont add duplicates to list
        foreach (NavNode navNode in visibleNavNode)
        {
            if(!m_visionNodes.Contains(navNode))
            {
                m_visionNodes.Add(navNode);
            }
        }

        //Build guard vision range
        foreach (NavNode navNode in m_visionNodes)
        {
            navNode.NPCVision(this, NavNode.ADD_REMOVE_FUNCTION.ADD);
        }
    }

    public Agent GetClosestTarget()
    {
        Agent possibleTarget = null;
        float targetDis = Mathf.Infinity;

        foreach (Agent agent in m_agentWorldState.GetPossibleTargets()) //TODO might have to update for multiple floors
        {
            if (agent == null || agent.m_knockedout)
                continue;

            float sqrDis = Vector3.SqrMagnitude(agent.transform.position - transform.position);
            if(sqrDis< targetDis)
            {
                targetDis = sqrDis;
                possibleTarget = agent;
            }
        }
        return possibleTarget;
    }


   
}
