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
                InteractSafe();
                //Enter Safe
            }
            //Extrance & Exit
            else
            {
                InteractDoor();
                //Enter Door
            }

        }
    }

    private void InteractSafe()
    {
        //Turns off particle
        safeOBJ.transform.GetChild(0).gameObject.SetActive(false);

        //Change mesh to open door
        GameObject safeDoor = safeOBJ.transform.GetChild(1).gameObject;
        Animator anim = safeDoor.GetComponent<Animator>();
        anim.SetTrigger("Safe_Open");

        //Completed that collider
        completed = true;
        transform.parent.GetComponent<KylesEndScene>().safeLooted = true;

        //Turns off collider
        GetComponent<Collider>().enabled = false;
    }

    private void InteractDoor()
    {
        //Completed that collider
        completed = true;
        transform.parent.GetComponent<KylesEndScene>().triggerEntered = true;
    }

}
