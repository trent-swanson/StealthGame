using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject {

	protected HashSet<KeyValuePair<string,object>> preconditions;
    protected HashSet<KeyValuePair<string,object>> effects;
 
    private bool inRange = false;
 
    public int cost = 1;

    public GameObject target = null;
 
    public Action() {
        preconditions = new HashSet<KeyValuePair<string, object>> ();
        effects = new HashSet<KeyValuePair<string, object>> ();
    }
 
    public void DoReset() {
        inRange = false;
        Reset ();
    }

    //Reset any variables that need to be reset before planning happens again.
    public abstract void Reset();
 
    //is the action done?
    public abstract bool IsDone();
 
    //Procedurally check if this action can run. Not all actions will need this, but some might.
    public abstract bool CheckProceduralPrecondition(GameObject agent, GameObject target);
 
    //Returns True if the action performed successfully
    //Happens in Update()
    public abstract void Perform(Agent agent);
 
    /**
     * Does this action need to be within range of a target game object?
     * If not then the moveTo state will not need to run for this action.
     */
    public abstract bool RequiresInRange ();
     
 
    /**
     * Are we in range of the target?
     * The MoveTo state will set this and it gets reset each time this action is performed.
     */
    public bool IsInRange () {
        return inRange;
    }
     
    public void setInRange(bool inRange) {
        this.inRange = inRange;
    }
 
 
    public void AddPrecondition(string key, object value) {
        preconditions.Add (new KeyValuePair<string, object>(key, value) );
    }
 
 
    public void RemovePrecondition(string key) {
        KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
        foreach (KeyValuePair<string, object> kvp in preconditions) {
            if (kvp.Key.Equals (key))
                remove = kvp;
        }
        if ( !default(KeyValuePair<string,object>).Equals(remove) )
            preconditions.Remove (remove);
    }
 
 
    public void AddEffect(string key, object value) {
        effects.Add (new KeyValuePair<string, object>(key, value) );
    }
 
 
    public void RemoveEffect(string key) {
        KeyValuePair<string, object> remove = default(KeyValuePair<string,object>);
        foreach (KeyValuePair<string, object> kvp in effects) {
            if (kvp.Key.Equals (key))
                remove = kvp;
        }
        if ( !default(KeyValuePair<string,object>).Equals(remove) )
            effects.Remove (remove);
    }
 
     
    public HashSet<KeyValuePair<string, object>> Preconditions {
        get {
            return preconditions;
        }
    }
 
    public HashSet<KeyValuePair<string, object>> Effects {
        get {
            return effects;
        }
    }
}