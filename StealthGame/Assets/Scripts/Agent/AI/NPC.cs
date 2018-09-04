using System.Collections;
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
        //Where agent is relative to node grid
        public Vector2Int m_currentNodePos= new Vector2Int(0, 0); 

        //Weapon information
        public enum WEAPON_TYPE { MELEE, RANGED }; //Fixed
        private WEAPON_TYPE m_weaponType = WEAPON_TYPE.MELEE; //Fixed

        //Node this agent wants to go to
        public NavNode m_targetNode = null; //Fixed

        //Seen targets
        public List<Agent> m_possibleTargets = new List<Agent>(); //Realtime

        //Targets which have gone missing
        public List<InvestigationNode> m_investigationNodes = new List<InvestigationNode>(); //Realtime

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




        if(m_GOAP.m_actionList.Count == 0)//Checking if at the end of the action list
        {
            bool newAction = m_GOAP.GOAPInit();

            if(!newAction)//Unable to get a new action
            {
                m_currentActionPoints = 0;
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
        base.AgentTurnEnd();
    }

    //Always updating view as this changes in real time TODO as it is turn based could move to check only when players move
    private void Update()
    {
        //Setup agents vals
        Vector3 checkOrigin = transform.position + transform.forward * m_colliderExtents.z + transform.up * m_colliderExtents.y;

        foreach (Agent oppposingAgent in m_opposingTeam)
        {
            if(!m_agentWorldState.m_possibleTargets.Contains(oppposingAgent))
            {
                //TODO sometimes can see players through walls, ray cast not working quite right

                //See if opposing agent is in vision cone
                Vector3 targetDir = oppposingAgent.transform.position - checkOrigin;

                Debug.DrawLine(checkOrigin, checkOrigin + targetDir * m_visionDistance);

                float dot = Vector3.Dot(targetDir.normalized, transform.forward);
                if (dot > 0.1f) //a bit less than 180 degrees of vision
                {
                    RaycastHit hit;

                    if (Physics.Raycast(checkOrigin, targetDir, out hit, m_visionDistance) && hit.collider.tag == "Player")
                    {
                        m_agentWorldState.m_possibleTargets.Add(oppposingAgent);
                    }
                }
            }
            else
            {
                Vector3 targetDir = oppposingAgent.transform.position - checkOrigin;
                Debug.DrawLine(checkOrigin, checkOrigin + targetDir * m_visionDistance, Color.red);
            }
        }    
    }

    public Agent GetClosestTarget()
    {
        Agent possibleTarget = null;
        float targetDis = Mathf.Infinity;

        foreach (Agent agent in m_agentWorldState.m_possibleTargets) //TODO might have to update for multiple floors
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
