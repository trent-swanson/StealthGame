using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_OpeningCamera : GameState
{
    public override bool UpdateState()
    {
        return true;
    }

    public override void StartState()
    {

    }

    public override void EndState()
    {
        //Update vision of Guards
        foreach (NPC NPCScript in GameObject.FindObjectsOfType<NPC>())
        {
            NPCScript.UpdateWorldState();
        }
       
    }

    public override bool IsValid()
    {
        return true;
    }
}
