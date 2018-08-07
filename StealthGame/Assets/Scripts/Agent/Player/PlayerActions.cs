using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private PlayerController m_playerController = null;
    private PlayerUI m_playerUI = null;
    private TurnManager m_turnManager = null;

    public NavNode m_currentSelectedNode = null;

    private List<NavNode> m_selectableNodes = new List<NavNode>();
    private List<NavNode> m_path = new List<NavNode>();

    private List<AnimationManager.AT> m_transitionSteps = new List<AnimationManager.AT>();

    public enum ACTION_STATE{TURN_START, VALID_NODE_SELECTION, INVALID_NODE_SELECTION, MOVING }
    public ACTION_STATE m_currentActionState = ACTION_STATE.TURN_START;

    private bool m_initActionState = true;

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_playerUI = GetComponent<PlayerUI>();
        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
    }

    //To run at the start of a players action
    public void InitActions()
    {
        GetAllSelectableNodes();
        NewActionState(ACTION_STATE.TURN_START);
    }

    //Basic Finate state machine setup
    public void UpdateActions()
    {
        switch (m_currentActionState) //TODO make good finate state machine
        {
            case ACTION_STATE.TURN_START:
                ActionStart();
                break;

            case ACTION_STATE.VALID_NODE_SELECTION:
                ValidSelection();
                break;

            case ACTION_STATE.INVALID_NODE_SELECTION:
                InvalidSelection();
                break;

            case ACTION_STATE.MOVING:
                Moving();
                break;

            default:
                break;
        }
    }

    //Start of a players action, need to draw navmesh, and get next step of action, be it valid or invalid node selection
    private void ActionStart()
    {
        if (m_initActionState)
        {
            m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.DRAW_NAVMESH, m_selectableNodes);
            m_initActionState = false;
        }

        NavNode newSelectedNavNode = GetMouseNode();
        if (newSelectedNavNode != null)
        {
            if (m_selectableNodes.Contains(newSelectedNavNode))
                NewActionState(ACTION_STATE.VALID_NODE_SELECTION);
            else
                NewActionState(ACTION_STATE.INVALID_NODE_SELECTION);
        }
    }

    public void ActionEnd()
    {
        m_playerUI.EndUI();
        m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.REMOVE_NAVMESH, m_selectableNodes);
    }

    //Valid node action, this is a node in the selectable list
    //When still valid, three options, draw path when new node is hovered over, on mouse down on valid node, start moving action, or on invalid selected change to invalid action
    private void ValidSelection()
    {
        NavNode newSelectedNavNode = GetMouseNode();
        if (newSelectedNavNode != null)
        {
            if(m_selectableNodes.Contains(newSelectedNavNode) && newSelectedNavNode.m_nodeState != NavNode.NODE_STATE.OBSTRUCTED)
            {
                if(newSelectedNavNode != m_currentSelectedNode)
                {
                    m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.DRAW_PATH, m_selectableNodes, m_currentSelectedNode, newSelectedNavNode, GetPath(newSelectedNavNode));
                    m_currentSelectedNode = newSelectedNavNode;
                }

                if(Input.GetMouseButtonDown(0))
                {
                    NewActionState(ACTION_STATE.MOVING);
                }
            }
            else
            {
                NewActionState(ACTION_STATE.INVALID_NODE_SELECTION);
            }
        }
        else
        {
            NewActionState(ACTION_STATE.INVALID_NODE_SELECTION);
        }
    }

    //Invalid action, this is a selected node which is obstructed, node is not selectable, or no node selected
    //Remove path and await valid selection
    private void InvalidSelection()
    {
        if (m_initActionState)
        {
            m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.REMOVE_PATH, m_selectableNodes, m_currentSelectedNode);
            m_currentSelectedNode = null;
            m_initActionState = false;
        }

        NavNode newSelectedNavNode = GetMouseNode();
        if (newSelectedNavNode != null && m_selectableNodes.Contains(newSelectedNavNode))
        {
            NewActionState(ACTION_STATE.VALID_NODE_SELECTION);
        }
    }

    //Moving action, Remove all UI for navmesh/ pathing, and path over to new selected node
    //After action go back to action start
    private void Moving()
    {
        if (m_initActionState)
        {
            m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.REMOVE_NAVMESH, m_selectableNodes);
            m_path = GetPath(m_currentSelectedNode);
            m_path.Reverse(); //Original path list goes from end to start
            m_path.RemoveAt(0); // dont need to move to starting pos
            m_playerController.m_currentNavNode.m_nodeState = NavNode.NODE_STATE.UNSELECTED; //Remove nodes obstructed status

            m_transitionSteps.Clear();
            m_transitionSteps = AnimationManager.GetTransistionSteps(m_playerController.m_currentNavNode, m_path);

            AnimationManager.SetupAnimator(m_playerController.m_animator, m_transitionSteps[0]);//Get initial aniamtion to play
            m_transitionSteps.RemoveAt(0);

            m_initActionState = false;
        }

        if (m_path.Count > 0)
        {
            bool atNextNode = MoveTo(m_path[0]);
            if(atNextNode)
            {
                m_playerController.m_currentNavNode = m_path[0];
                m_path.RemoveAt(0);

                if(m_transitionSteps.Count > 0)
                {
                    AnimationManager.SetupAnimator(m_playerController.m_animator, m_transitionSteps[0]);
                    m_transitionSteps.RemoveAt(0);
                }

                if (m_path.Count == 0)//End of move
                {
                    m_playerController.m_currentActionPoints = m_currentSelectedNode.m_BFSDistance;//Set action points to node value
                    m_playerController.m_currentNavNode.m_nodeState = NavNode.NODE_STATE.OBSTRUCTED;
                    InitActions();
                }
            }
        }
    }

    //Find what type of action is about to be take

    //Change current action, resets action init bool to true, TODO in proper finate state machine this will be managed by a manager so no bool is needed
    private void NewActionState(ACTION_STATE actionState)
    {
        m_currentActionState = actionState;
        m_initActionState = true;
    }
    
    //The currently selcted nav node based from the mouse
    private NavNode GetMouseNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))
        {
            return(hit.collider.GetComponent<NavNode>());
        }

        return null;
    }

    private void GetAllSelectableNodes()
    {
        m_selectableNodes.Clear();

        NavNode currentNavNode = m_playerController.m_currentNavNode;

        currentNavNode.m_BFSDistance = m_playerController.m_currentActionPoints;
        currentNavNode.m_BFSPreviousNode = null;

        Queue<NavNode> BFSQueue = new Queue<NavNode>();
        BFSQueue.Enqueue(currentNavNode);
        NavNode currentBFSNode = null;

        while (BFSQueue.Count > 0) //BFS implementation
        {
            currentBFSNode = BFSQueue.Dequeue();
            m_selectableNodes.Add(currentBFSNode);

            foreach (NavNode nextBFSNode in currentBFSNode.m_adjacentNodes)
            {
                if (!m_selectableNodes.Contains(nextBFSNode) && !BFSQueue.Contains(nextBFSNode) && nextBFSNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE) //TODO do we want to move through players, if not add nextBFSNode.m_nodeState != NavNode.NODE_STATE.OBSTRUCTED
                {
                    // int distance = currentBFSNode.m_BFSDistance - 1 - (Mathf.Abs(nextBFSNode.m_gridPos.y - currentBFSNode.m_gridPos.y));
                    int distance = currentBFSNode.m_BFSDistance - 1;

                    nextBFSNode.m_BFSDistance = distance;
                    nextBFSNode.m_BFSPreviousNode = currentBFSNode;

                    if (distance >= 0)
                        BFSQueue.Enqueue(nextBFSNode);
                }
            }
        }
    }

    //Get path with first obect in list being the end node
    private List<NavNode> GetPath(NavNode endNode)
    {
        List<NavNode> path = new List<NavNode>();

        NavNode currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_BFSPreviousNode;
        }

        return path;
    }

    private bool MoveTo(NavNode pathNode)
    {
        Vector3 targetPos = m_path[0].m_nodeTop;
        Vector3 velocityVector = targetPos - transform.position;
        float translateDis = velocityVector.magnitude;

        velocityVector = velocityVector.normalized * Time.deltaTime * m_playerController.m_moveSpeed;

        if (velocityVector.magnitude > translateDis)//Arrived at node
        {
            transform.position = targetPos;
            m_playerController.m_currentNavNode = m_path[0];
            return true;
        }
        else
        {
            transform.position += velocityVector;
        }
        return false;
    }
}
