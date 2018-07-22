using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GOAPPathfinding
{
    public class Node
    {
        public Action m_action = null;
        public bool m_isValid = true;
        public List<Action> m_potentialActions = new List<Action>(); //What actions can effect this nodes validity states
        public List<WorldState.WORLD_STATE> m_validityStates = new List<WorldState.WORLD_STATE>(); //What states are needed to validate this node
        public List<Node> m_satifiedNodes = new List<Node>(); //What actions have effected validity
        public Node m_previousNode = null;

        public float GetCost()
        {
            float cost = GetSatisfiedCosts(this);

            if(m_previousNode != null) // In the case this branch had  previous parent which relys on other nodes to function
            {
                //Get to top of tree
                Node topNode = this;
                while (topNode.m_previousNode.m_previousNode != null)
                    topNode = topNode.m_previousNode;

                cost += topNode.GetSatisfiedCosts(topNode);

            }
            return cost;
        }

        public float GetSatisfiedCosts(Node topNode)
        {
            float cost = topNode.m_action.ActionCost;
            foreach (Node node in topNode.m_satifiedNodes)
            {
                cost += node.GetSatisfiedCosts(node);
            }
            return cost;
        }

        public void UpdateNodeValidity(Node validNode)
        {
            foreach (WorldState.WORLD_STATE worldState in validNode.m_action.SatisfiedWorldStates)
                m_validityStates.Remove(worldState);

            m_satifiedNodes.Add(validNode);

            if(m_validityStates.Count==0)
            {
                m_isValid = true;
                if(m_previousNode!= null)
                    m_previousNode.UpdateNodeValidity(this);
            }
        }
    }

    public static List<Action> GetActionPlan(GameObject agent, WorldState.WORLD_STATE goalState, List<Action> possibleActions)
    {
        Node startingNode = InitialNode(agent, goalState, possibleActions);

        if (startingNode.m_isValid)
            return new List<Action>();

        Node currentNode = startingNode;
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        AddNextNodes(agent, currentNode, possibleActions, openNodes, closedNodes);

        //Loop till no more options
        while (openNodes.Count > 0)
        {
            currentNode = GetLowestFScore(openNodes);
            AddNextNodes(agent, currentNode, possibleActions, openNodes, closedNodes);

            //Break early when at end
            if (startingNode.m_isValid)
                return GetPath(currentNode, startingNode);
        }
        return new List<Action>();
    }

    private static bool IsActionValid(GameObject agent, Action action)
    {
        foreach (WorldState.WORLD_STATE worldState in action.RequiredWorldStates)
        {
            if (!WorldState.CheckForValidState(agent, worldState))
                return false;
        }
        return true;
    }

    private static void AddNextNodes(GameObject agent, Node currentNode, List<Action> possibleActions, List<Node> openNodes, List<Node> closedNodes)
    {
        foreach (Action newActionNode in currentNode.m_potentialActions)
        {
            Node newNode = NewNode(agent, possibleActions, newActionNode, currentNode);
            openNodes.Add(newNode);

            //If new node is already valid then notify previous parent
            if (newNode.m_isValid)
                currentNode.UpdateNodeValidity(newNode);
        }
        openNodes.Remove(currentNode);
        closedNodes.Add(currentNode);
    }

    private static Node GetLowestFScore(List<Node> openNodes)
    {
        float fScore = Mathf.Infinity;
        Node highestFNode = null;
        foreach (Node node in openNodes)
        {
            if (node.GetCost() < fScore)
            {
                highestFNode = node;
                fScore = node.GetCost();
            }
        }
        return highestFNode;
    }

    private static List<Action> GetPath(Node currentNode, Node startingNode)
    {
        //Go to top most of formed tree
        while (currentNode.m_previousNode != startingNode)
        {
            currentNode = currentNode.m_previousNode;
        }
        
        //go through tree from bottom to top
        return GetPreceedingActions(currentNode);
    }

    private static List<Action> GetPreceedingActions(Node topNode)
    {
        List<Action> preceedingActions = new List<Action>();
        foreach (Node lowerNode in topNode.m_satifiedNodes)
        {
            preceedingActions.AddRange(GetPreceedingActions(lowerNode));
        }
        preceedingActions.Add(topNode.m_action);
        return preceedingActions;
    }

    private static Node InitialNode(GameObject agent, WorldState.WORLD_STATE worldState, List<Action> possibleActions)
    {
        //Create new node
        Node newNode = new Node();

        //Add all actions that satify the required world states for a given action which are not currently valid
        if (!WorldState.CheckForValidState(agent, worldState))
        {
            newNode.m_isValid = false; // At least one state has not been satified so is considered not valid action
            newNode.m_validityStates.Add(worldState);

            foreach (Action action in possibleActions)
            {
                if (action.SatisfiedWorldStates.Contains(worldState) && !newNode.m_potentialActions.Contains(action))
                    newNode.m_potentialActions.Add(action);
            }
        }

        return newNode;
    }

    private static Node NewNode(GameObject agent, List<Action> possibleActions, Action newAction, Node previousNode)
    {
        //Create new node
        Node newNode = new Node();
        newNode.m_action = newAction;
        newNode.m_previousNode = previousNode;
        //Add all actions that satify the required world states for a given action which are not currently valid
        foreach (WorldState.WORLD_STATE worldState in newAction.RequiredWorldStates)
        {
            newNode.m_isValid = false; // At least one state has not been satified so is considered not valid action
            newNode.m_validityStates.Add(worldState);

            foreach (Action action in possibleActions)
            {
                if (action.SatisfiedWorldStates.Contains(worldState) && !newNode.m_potentialActions.Contains(action))
                    newNode.m_potentialActions.Add(action);
            }
        }

        return newNode;
    }
}
