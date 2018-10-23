using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Agent
{
    public GOAP m_GOAP = null;

    public PathCreator m_pathCreator = null;

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
        public void SetPossibleTargets(List<Agent> possibleTargets) {m_possibleTargets = possibleTargets; m_modifiedFlag = true;}
        public List<Agent> GetPossibleTargets() { return m_possibleTargets; }

        //Targets which have gone missing
        [SerializeField]
        private List<InvestigationNode> m_investigationNodes = new List<InvestigationNode>(); //Realtime
        public void SetInvestigationNodes(List<InvestigationNode> investigationNodes) { m_investigationNodes = investigationNodes; m_modifiedFlag = true; }
        public List<InvestigationNode> GetInvestigationNodes() { return m_investigationNodes; }

        //Waypoints for patrolling
        [SerializeField]
        public List<NavNode> m_waypoints = new List<NavNode>();
        public int m_waypointIndex = 0;
    }

    [System.Serializable]
    public struct InvestigationNode
    {
        public Agent m_target;
        public NavNode m_node;
    }

    //Node this agent wants to go to
    public NavNode m_targetNode = null; //Fixed

    public List<NavNode> m_visionNodes = new List<NavNode>();

    [Space]
    [Header("AgentState")]
    public AgentWorldState m_agentWorldState;

    [Space]
    public int m_autoStandupTimer = 0;
    public SpriteRenderer m_alertIcon = null;


    protected override void Start()
    {
        base.Start();

        m_GOAP = GetComponent<GOAP>();

        if (m_pathCreator != null)
            m_path = m_pathCreator.m_path;
    }


    //Start of turn, only runs once per turn
    public override void AgentTurnInit()
    {
        base.AgentTurnInit();
    }

    //Runs every time a agent is selected, this can be at end of an action is completed
    public override void AgentSelected()
    {
        m_cameraController.Focus(transform);
        UpdateWorldState();
    }

    //Constant update while agent is selected
    public override AGENT_UPDATE_STATE AgentTurnUpdate()
    {
        if (!m_agentAnimationController.m_playNextAnimation) //Currently animating, dont need any logic
            return AGENT_UPDATE_STATE.PERFORMING_ACTIONS;

        if (m_currentActionPoints == 0) //Early break on no aciotn points
            return AGENT_UPDATE_STATE.END_TURN;

        //Check for update in world state
        if (m_agentWorldState.m_modifiedFlag)
        {
            m_agentWorldState.m_modifiedFlag = false;
            m_GOAP.GOAPInit();
        }

        if (m_GOAP.m_actionList.Count == 0)//Checking if at the end of the action list
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
                m_GOAP.m_actionList.RemoveAt(0);
                return AGENT_UPDATE_STATE.PERFORMING_ACTIONS;
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

        bool modifiedPossibleTargets = false;
        List<Agent> possibleTargets = m_agentWorldState.GetPossibleTargets();
        bool modifiedInvestigationTargets = false;
        List<InvestigationNode> investigationNodes = m_agentWorldState.GetInvestigationNodes();

        foreach (NavNode navNode in m_visionNodes)//Check for player in vision
        {
            Agent obstructingAgent = navNode.m_obstructingAgent;
            if (obstructingAgent != null && obstructingAgent.m_team != m_team)//Vision node has enemy agent on it
            {
                if (!possibleTargets.Contains(obstructingAgent))
                {
                    //Add player to possible targets
                    possibleTargets.Add(obstructingAgent);

                    //Remove any investigation nodes
                    for (int i = 0; i < investigationNodes.Count; i++)
                    {
                        if(investigationNodes[i].m_target == obstructingAgent)
                        {
                            investigationNodes.RemoveAt(i);
                            i--;
                        }
                    }
                    modifiedPossibleTargets = true;
                }
            }
        }

        for (int possibleTargetIndex = 0; possibleTargetIndex < possibleTargets.Count; possibleTargetIndex++)//See if player has left vision
        {
            if(!m_visionNodes.Contains(possibleTargets[possibleTargetIndex].m_currentNavNode) && !possibleTargets[possibleTargetIndex].m_knockedout) //Player left view and is not not knocked out
            {
                for (int investigationNodeIndex = 0; investigationNodeIndex < investigationNodes.Count; investigationNodeIndex++)//Has node already been added
                {
                    if (investigationNodes[investigationNodeIndex].m_target == possibleTargets[possibleTargetIndex])
                    {
                        investigationNodes.RemoveAt(investigationNodeIndex);
                        investigationNodeIndex--;
                    }
                }

                InvestigationNode newInvestigation;

                newInvestigation.m_node = possibleTargets[possibleTargetIndex].m_currentNavNode;
                newInvestigation.m_target = possibleTargets[possibleTargetIndex];
                investigationNodes.Add(newInvestigation);

                possibleTargets.RemoveAt(possibleTargetIndex);
                possibleTargetIndex--;

                modifiedInvestigationTargets = true;
                modifiedPossibleTargets = true;
            }
        }

        if (modifiedPossibleTargets)
        {
            m_agentWorldState.SetPossibleTargets(possibleTargets);
        }
        if (modifiedInvestigationTargets)
        {
            m_agentWorldState.SetInvestigationNodes(investigationNodes);
        }

        ToggleAlertIcon();
    }

    public void BuildVision()
    {
        if (m_knockedout)//Early breakout for any knocked out units
            return;

        foreach (NavNode navNode in m_visionNodes) //Remove old vision
        {
            navNode.NPCVision(ADD_REMOVE_FUNCTION.REMOVE, this);
        }

        m_visionNodes.Clear();

        m_visionNodes = Vision.BuildVisionList(this, m_visionFullDistance, m_visionFullAngle);

        //Build vision cone, dont add duplicates to list
        foreach (NavNode navNode in m_visionNodes)
        {
            navNode.m_NPCVisionUI.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, m_visionFullOpacity);
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

    public override void Knockout()
    {
        base.Knockout();

        foreach (NavNode navNode in m_visionNodes) //Remove old vision
        {
            navNode.NPCVision(ADD_REMOVE_FUNCTION.REMOVE, this);
        }

        m_currentActionPoints = 0;
        m_autoStandupTimer = m_NPCTurn.m_autoStandupTime;
        ToggleAlertIcon();
    }

    public void ToggleAlertIcon()
    {
        m_alertIcon.enabled = !m_knockedout && (m_agentWorldState.GetPossibleTargets().Count > 0 || m_agentWorldState.GetInvestigationNodes().Count > 0);
    }
}
