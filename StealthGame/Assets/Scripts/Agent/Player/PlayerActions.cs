using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActions : MonoBehaviour
{
    private AgentAnimationController m_agentAnimationController = null;
    private PlayerController m_playerController = null;
    private PlayerUI m_playerUI = null;

    public NavNode m_currentSelectedNode = null;

    private List<NavNode> m_selectableNodes = new List<NavNode>();

    public enum ACTION_STATE{ACTION_START, VALID_NODE_SELECTION, WALL_HIDE_SELECTION, INVALID_NODE_SELECTION, ACTION_PERFORM }
    public ACTION_STATE m_currentActionState = ACTION_STATE.ACTION_START;

    private bool m_initActionState = true;
    
    [Tooltip("How far the mouse will go off a node while keeping the wall hide icons")]
    public static float m_wallHidingDistance = 2.5f;

    private void Start()
    {
        m_agentAnimationController = GetComponent<AgentAnimationController>();
        m_playerController = GetComponent<PlayerController>();
        m_playerUI = GetComponent<PlayerUI>();
    }

    //To run at the start of a players action
    public void InitActions()
    {
        GetAllSelectableNodes();
        NewActionState(ACTION_STATE.ACTION_START);
    }

    //Basic Finate state machine setup
    public void UpdateActions()
    {
        switch (m_currentActionState) //TODO make good finate state machine
        {
            case ACTION_STATE.ACTION_START:
                ActionStart();
                break;

            case ACTION_STATE.VALID_NODE_SELECTION:
                ValidSelection();
                break;

            case ACTION_STATE.INVALID_NODE_SELECTION:
                InvalidSelection();
                break;

            case ACTION_STATE.WALL_HIDE_SELECTION:
                WallHideSelection();
                break;

            case ACTION_STATE.ACTION_PERFORM:
                ActionPerform();
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
        if (newSelectedNavNode != null) // Hovering over empty space
        {
            if(newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE)
                newSelectedNavNode.UpdateWallIndicators();

            Agent downedAgent = newSelectedNavNode.GetDownedAgent(m_playerController.m_team);

            bool nextNodeUsable = newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE || //Node is walkable
                newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE || //Interaction node
                (newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED &&
                newSelectedNavNode.m_obstructingAgent != null && 
                newSelectedNavNode.m_obstructingAgent.m_team != m_playerController.m_team) || //Node contains an enemy 
                downedAgent != null;//Node contains downed ally

            if (m_selectableNodes.Contains(newSelectedNavNode) && nextNodeUsable)
            {
                if(newSelectedNavNode != m_currentSelectedNode)
                {
                    m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.DRAW_PATH, m_selectableNodes, m_currentSelectedNode, newSelectedNavNode, GetPath(newSelectedNavNode));
                    m_currentSelectedNode = newSelectedNavNode;
                }

                if(Input.GetMouseButtonDown(0))
                {
                    //TODO get action from node e.g. use node, attack
                    NewActionState(ACTION_STATE.ACTION_PERFORM);
                }
            }
            else // hovering over unreachable nav node
            {
                if(newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.LOW_OBSTACLE || newSelectedNavNode.m_nodeType == NavNode.NODE_TYPE.HIGH_OBSTACLE)
                    NewActionState(ACTION_STATE.WALL_HIDE_SELECTION);
                else
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

    private void WallHideSelection()
    {
        if(m_currentSelectedNode == null)
            NewActionState(ACTION_STATE.INVALID_NODE_SELECTION);
        else
        {
            m_currentSelectedNode.UpdateWallIndicators();

            NavNode newSelectedNavNode = GetMouseNode();
            if (newSelectedNavNode != null)
            {
                if (m_selectableNodes.Contains(newSelectedNavNode)) //Moving back to hovering over valid tile
                    NewActionState(ACTION_STATE.VALID_NODE_SELECTION);
                else if (Input.GetMouseButtonDown(0)) //Selecting wall hide
                    NewActionState(ACTION_STATE.ACTION_PERFORM);
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))//Dont raycast when over UI
                    {
                        Vector3 collisionPoint = hit.point;

                        if (Vector3.Distance(collisionPoint, m_currentSelectedNode.m_nodeTop) > m_wallHidingDistance)
                            NewActionState(ACTION_STATE.INVALID_NODE_SELECTION);
                    }
                }
            }
            else
                NewActionState(ACTION_STATE.INVALID_NODE_SELECTION);
        }
    }

    //Moving action, Remove all UI for navmesh/ pathing, and path over to new selected node
    //After action go back to action start
    private void ActionPerform()
    {
        if (m_initActionState)
        {
            m_playerController.m_currentActionPoints = m_currentSelectedNode.m_BFSDistance;//Set action points to node value

            //Get animation steps
            m_agentAnimationController.m_animationSteps.Clear();

            if (m_playerController.m_interaction == INTERACTION_TYPE.WALL_HIDE)//Previously hiding on wall, so defualt by adding idle
            {
                m_agentAnimationController.m_animationSteps.Add(AnimationManager.ANIMATION_STEP.IDLE);
            }

            m_playerController.m_interaction = INTERACTION_TYPE.NONE;//Reset interaction

            //Getting wall hide detection
            FACING_DIR wallHideDir = m_currentSelectedNode.GetWallHideDir();

            if (wallHideDir != FACING_DIR.NONE)//Wall hiding animation calling
            {
                m_playerController.m_interaction = INTERACTION_TYPE.WALL_HIDE;
            }
            else if (m_currentSelectedNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED)//Attacking as were moving to a obstructed tile
            {
                m_playerController.m_interaction = INTERACTION_TYPE.ATTACK;
                m_playerController.m_targetAgent = m_currentSelectedNode.m_obstructingAgent;

                m_playerController.m_path = GetPath(m_currentSelectedNode);
                m_playerController.m_path.RemoveAt(m_playerController.m_path.Count - 1); //As were attackig no need to move to last tile
            }
            else if (m_currentSelectedNode.m_item != null)//Picking up item
            {
                m_playerController.m_interaction = INTERACTION_TYPE.PICKUP_ITEM;
                m_playerController.m_targetItem = m_currentSelectedNode.m_item;

                m_playerController.m_path = GetPath(m_currentSelectedNode);
            }
            else if (m_currentSelectedNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE)//Using interactable
            {
                m_playerController.m_interaction = INTERACTION_TYPE.INTERACTABLE;
                m_playerController.m_targetInteractable = m_currentSelectedNode.m_interactable;

                //Get new path to interactable node
                m_playerController.m_path = GetPath(m_currentSelectedNode.m_interactable.GetInteractableNode(m_playerController));
            }
            else if (m_currentSelectedNode.GetDownedAgent(m_playerController.m_team) != null)//Reviving team mate
            {
                m_playerController.m_interaction = INTERACTION_TYPE.REVIVE;
                m_playerController.m_targetAgent = m_currentSelectedNode.GetDownedAgent(m_playerController.m_team);

                m_playerController.m_path = GetPath(m_currentSelectedNode);
                m_playerController.m_path.RemoveAt(m_playerController.m_path.Count - 1); //As were reviving no need to move to last tile
            }
            else //Default to jst walk to node
            {
                m_playerController.m_path = GetPath(m_currentSelectedNode);
            }

            transform.position = m_playerController.m_path[0].m_nodeTop;//Move to top of node to remove any minor offsets due to float errors

            m_agentAnimationController.m_animationSteps.AddRange(AnimationManager.GetAnimationSteps(this.m_playerController, m_playerController.m_path, m_playerController.m_interaction, wallHideDir));

            m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.REMOVE_NAVMESH, m_selectableNodes, m_currentSelectedNode);//Remove UI

            m_initActionState = false;
        }

        if(m_agentAnimationController.m_playNextAnimation)//End of animation
        {
            m_agentAnimationController.PlayNextAnimation();
            m_playerUI.UpdateUI();

            if (m_agentAnimationController.m_animationSteps.Count == 0)//End of move
            {
                m_playerController.ChangeCurrentNavNode(m_playerController.m_path[m_playerController.m_path.Count - 1]);

                InitActions();
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
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))//Dont raycast when over UI
        {
            return (hit.collider.GetComponent<NavNode>());
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
                if (!m_selectableNodes.Contains(nextBFSNode) && !BFSQueue.Contains(nextBFSNode))
                {
                    int distance = currentBFSNode.m_BFSDistance - 1;

                    nextBFSNode.m_BFSDistance = distance;
                    nextBFSNode.m_BFSPreviousNode = currentBFSNode;

                    if (distance >= 0)
                    {
                        if(nextBFSNode.m_nodeType == NavNode.NODE_TYPE.WALKABLE)//TODO if we want to move through team mates just compare team values
                            BFSQueue.Enqueue(nextBFSNode);
                        else if (nextBFSNode.m_nodeType == NavNode.NODE_TYPE.OBSTRUCTED || nextBFSNode.m_nodeType == NavNode.NODE_TYPE.INTERACTABLE)
                            m_selectableNodes.Add(nextBFSNode);
                    }
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
        path.Reverse();//Path is back to front when created
        return path;
    }
}
