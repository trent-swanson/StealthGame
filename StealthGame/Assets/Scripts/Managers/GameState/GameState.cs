using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public List<GameState> m_nextStates = new List<GameState>();

    /// <returns>True when state is completed</returns>
    public virtual bool UpdateState()
    {
        return true;
    }

    public virtual void StartState()
    {

    }

    public virtual void EndState()
    {

    }

    public virtual bool IsValid()
    {
        return true;
    }
}
