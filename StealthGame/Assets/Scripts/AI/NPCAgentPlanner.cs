using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAgentPlanner : MonoBehaviour
{
    public struct InvestigationNode
    {
        private GameObject m_target;
        public GameObject Target
        {
            get { return m_target; }
            set { m_target = value; }
        }
        private GameObject m_node;
        public GameObject Node
        {
            get { return m_node; }
            set { m_node = value; }
        }
    }

    //-----------------------
    // Agent States
    //-----------------------

    //Weapon information
    public enum WEAPON_TYPE {MELEE, RANGED }; //Fixed
    private WEAPON_TYPE m_weaponType = WEAPON_TYPE.MELEE; //Fixed
    public WEAPON_TYPE WeaponType
    {
        get { return m_weaponType; }
        set { m_weaponType = value; }
    }

    //Node this agent wants to go to
    private GameObject m_targetNode = null; //Fixed
    public GameObject TargetNode
    {
        get { return m_targetNode; }
        set { m_targetNode = value; }
    }

    //Seen targets
    private List<GameObject> m_possibleTargets = new List<GameObject>(); //Realtime
    public List<GameObject> PossibleTargets
    {
        get { return m_possibleTargets; }
        set { m_possibleTargets = value; }
    }

    //Targets which have gone missing
    private List<InvestigationNode> m_investigationNodes = new List<InvestigationNode>(); //Realtime
    public List<InvestigationNode> InvestigationNodes
    {
        get { return m_investigationNodes; }
        set { m_investigationNodes = value; }
    }

    //Waypoints
    [SerializeField]
    private List<GameObject> m_waypoints = new List<GameObject>();
    public List<GameObject> Waypoints
    {
        get { return m_waypoints; }
        set { m_waypoints = value; }
    }

    private int m_waypointIndex = 0;
    public int WaypointIndex
    {
        get { return m_waypointIndex; }
        set { m_waypointIndex = value; }
    }

    //Common data usage
    private List<GameObject> m_opposingTeam = new List<GameObject>();
    private NPCAgent m_NPCAgent = null;

    private List<Action> m_actionPlan = new List<Action>();
    private int m_actionPlanIndex = 0;

    //--------------------------------------------------------------------------------------
    // Initilisation
    // Setup all varibles for constant usage
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        //TODO get all opposing team agents
        List<Agent> opposingAgents = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>().m_playerTeam;
        foreach (Agent agent in opposingAgents)
        {
            m_opposingTeam.Add(agent.gameObject);
        }

        m_NPCAgent = GetComponent<NPCAgent>();
    }

    public void StartUnitTurn()
    {
        //Determine Goal TODO, based off priority override, when one fails loop to next etc
        Goal currentGoal = m_NPCAgent.PossibleGoals[0];

        m_actionPlan = GOAPPathfinding.GetActionPlan(gameObject, currentGoal.DesiredWorldState, m_NPCAgent.PossibleActions);
        m_actionPlanIndex = 0;

        //Setup first action
        if (m_actionPlan.Count > 0)
            m_actionPlan[0].ActionInit(gameObject);
        else
            m_NPCAgent.TurnEnd();//No actions found
    }

    public void TurnUpdate()
    {
        //Run action
        m_actionPlan[m_actionPlanIndex].Perform(gameObject);
        //Check if done
        if(m_actionPlan[m_actionPlanIndex].IsDone(gameObject))
        {
            m_actionPlan[m_actionPlanIndex].EndAction(gameObject);
            m_actionPlanIndex++;
            if (m_actionPlanIndex == m_actionPlan.Count)
                m_NPCAgent.TurnEnd();//All actions completed
            else
                m_actionPlan[m_actionPlanIndex].ActionInit(gameObject);
        }
    }

    //--------------------------------------------------------------------------------------
    // Update the states for NPC Agents in "Realtime", that is each action the player takes
    // Not on each update frame
    //--------------------------------------------------------------------------------------
    private void UpdateRealtimeStates()
    {
        CheckLineOfSight();
    }

    //--------------------------------------------------------------------------------------
    // Check for line of sight on opposing team
    // On sight, add to seen list
    // Upon loosing sight add to investigation list
    //--------------------------------------------------------------------------------------
    private void CheckLineOfSight()
    {
        foreach (GameObject opposingAgent in m_opposingTeam)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, opposingAgent.transform.position - transform.position, out hit);

            if(m_possibleTargets.Contains(opposingAgent))
            {
                if (hit.collider.gameObject != opposingAgent)
                {
                    m_possibleTargets.Remove(opposingAgent);

                    //Update investigation Nodes
                    for (int i = 0; i < m_investigationNodes.Count; i++)
                    {
                        if (m_investigationNodes[i].Target == opposingAgent)
                        {
                            //Cant change struct property directly in c#
                            InvestigationNode investigationNode = m_investigationNodes[i];
                            investigationNode.Node = opposingAgent.GetComponent<Agent>().GetNodeBelow();
                            m_investigationNodes[i] = investigationNode;

                            break;
                        }
                    }

                }
            }
            else if (hit.collider.gameObject == opposingAgent)
            {
                for (int i = 0; i < m_investigationNodes.Count; i++)
                {
                    if (m_investigationNodes[i].Target == opposingAgent)
                    {
                        m_investigationNodes.RemoveAt(i);
                        break;
                    }
                }

                m_possibleTargets.Add(opposingAgent);
            }
        }
    }

    public void GetClosesetTarget(ref GameObject closestTarget, ref float distance)
    {
        if(m_possibleTargets.Count > 0)
        {
            closestTarget = m_possibleTargets[0];
            float dis = Vector3.Distance(closestTarget.transform.position, transform.position);
            for (int i = 1; i < m_possibleTargets.Count; i++)
            {
                float newDis = Vector3.Distance(m_possibleTargets[i].transform.position, transform.position);
                if(newDis < dis)
                {
                    dis = newDis;
                    closestTarget = m_possibleTargets[i];
                }
            }
        }
    }

    public GameObject GetNodeBelow(GameObject agent)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.0f))
        {
            if (hit.transform.tag == "Tile")
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }
}
