using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState m_currentState = null;
    public PlayerState m_turnStartingState = null;

    public NavNode m_currentSelectedNode = null;
    public bool m_currentlyHiding = false;

    public List<NavNode> m_selectableNodes = new List<NavNode>();

    private PlayerController m_playerController = null;
    private PlayerUI m_playerUI = null;

    public void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_playerUI = GetComponent<PlayerUI>();

        //Initilise all states
        PlayerState[] m_playerStates = GetComponentsInChildren<PlayerState>();

        for (int i = 0; i < m_playerStates.Length; i++)
        {
            m_playerStates[i].StateInit();
        }

        //Run first state
        m_currentState.StateStart();
    }

    public void TurnStartStateMachine()
    {
        SwapState(m_turnStartingState);
    }

    public void TurnEndStateMachine()
    {
        m_playerUI.EndUI();
        m_playerUI.UpdateNodeVisualisation(PlayerUI.MESH_STATE.REMOVE_NAVMESH, m_selectableNodes);
    }


    public void UpdateStateMachine()
    {
        if (m_currentState.UpdateState())
        {
            //Find next valid state
            foreach (PlayerState playerState in m_currentState.m_nextStates)
            {
                if (playerState.IsValid())
                {
                    SwapState(playerState);
                    return; //Early break out
                }
            }

            //No valid states and current state is not valid, default to the defualt state
            if (m_currentState.m_defaultState != null && !m_currentState.IsValid())
            {
                SwapState(m_currentState.m_defaultState);
            }
        }
    }

    private void SwapState(PlayerState p_nextState)
    {
        if(m_currentState!= null)
            m_currentState.StateEnd();
        m_currentState = p_nextState;
        m_currentState.StateStart();
    }

    //The currently selcted nav node based from the mouse
    public NavNode GetMouseNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))//Dont raycast when over UI
        {
            return (hit.collider.GetComponent<NavNode>());
        }

        return null;
    }

    //Get path with first obect in list being the end node
    public List<NavNode> GetPath(NavNode endNode)
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
