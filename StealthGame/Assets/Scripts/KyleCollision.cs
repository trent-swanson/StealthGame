using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyleCollision : MonoBehaviour {

    public bool completed = false;
    public List<GameObject> inside = new List<GameObject>();

    public bool safe = false;
    public GameObject safeOBJ;

    void Start()
    {
        //Checks if this is the Safe
        if (name == "Safe")
        {
            safe = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If player
        if (other.tag == "Player")
        {
            //Adds player to this object's list
            inside.Add(other.gameObject);

            //Safe
            if (safe)
            {
                InteractSafe(other);
                //Enter Safe
                Debug.Log("Entered Safe");
            }
            //Extrance & Exit
            else
            {
                InteractDoor();
                //Enter Door
                Debug.Log("Entered Door");
            }

        }
    }

    private void InteractSafe(Collider other)
    {
        //Turns off particle
        other.transform.GetChild(1).gameObject.SetActive(false);
        //Change mesh to open door
        safeOBJ.GetComponent<MeshRenderer>().enabled = false;
        //^^^REPLACE^^^

        //Completed that collider
        completed = true;
        transform.parent.GetComponent<KylesEndScene>().safeLooted = true;
    }

    private void InteractDoor()
    {
        //Completed that collider
        completed = true;
        transform.parent.GetComponent<KylesEndScene>().triggerEntered = true;
    }

}
