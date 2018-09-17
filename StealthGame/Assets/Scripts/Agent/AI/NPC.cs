using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Agent
{
    public GOAP m_GOAP = null;

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

        //Waypoints for patrolling
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

        m_GOAP = GetComponent<GOAP>();
        m_opposingTeam = m_turnManager.GetOpposingTeam(m_team);
    }


    //Start of turn, only runs once per turn
    public override void AgentTurnInit()
    {
        base.AgentTurnInit();

        if(m_knockedout)
        {
            m_autoStandupTimer--;
            if(m_autoStandupTimer<=0)
            {
                Revive();
            }
        }
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public override void AgentSelected()
    {
        
    }

    //Constant update while agent is selected
    public override AGENT_UPDATE_STATE AgentTurnUpdate()
    {
        //Check for update in world state
        if(m_agentWorldState.m_modifiedFlag)
        {
            m_agentWorldState.m_modifiedFlag = false;
        }

        if(m_GOAP.m_actionList.Count == 0)//Checking if at the end of the action list
        {
            bool newAction = m_GOAP.GOAPInit();

            if(!newAction)//Unable to get a new action
            {
                m_currentActionPoints = 0;
                return AGENT_UPDATE_STATE.END_TURN;
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
                return AGENT_UPDATE_STATE.PERFORMING_ACTIONS;
            default:
                break;
        }

        return AGENT_UPDATE_STATE.AWAITING_INPUT;
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
            navNode.NPCVision(ADD_REMOVE_FUNCTION.REMOVE, this);
        }

        m_visionNodes.Clear();

        List<NavNode> fullVisibleNavNode = Vision.BuildVisionList(this, m_visionFullDistance, m_visionFullAngle);
        List<NavNode> fadeVisibleNavNode = Vision.BuildVisionList(this, m_visionFadeDistance, m_visionFadeAngle);

        //Build vision cone, dont add duplicates to list
        foreach (NavNode navNode in fullVisibleNavNode)
        {
            fadeVisibleNavNode.Remove(navNode);
            navNode.m_NPCVisionUI.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, m_visionFullOpacity);
        }

        foreach (NavNode navNode in fadeVisibleNavNode)
        {
            float navNodeDistance = Vector3.Distance(navNode.m_nodeTop, m_currentNavNode.m_nodeTop);
            float opacticy = m_visionFadeMinOpacity + ((m_visionFadeMaxOpacity - m_visionFadeMinOpacity) * ((m_visionFadeDistance - navNodeDistance) / (m_visionFadeDistance - m_visionFullDistance)));
            navNode.m_NPCVisionUI.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacticy);
        }

        m_visionNodes.AddRange(fullVisibleNavNode);
        m_visionNodes.AddRange(fadeVisibleNavNode);

        //Build guard vision range
        foreach (NavNode navNode in m_visionNodes)
        {
            navNode.NPCVision(ADD_REMOVE_FUNCTION.ADD, this);
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
