using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temptesting : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        StartCoroutine(test());
        StartCoroutine(test2());
        StartCoroutine(test3());
    }


    private IEnumerator test()
    {
        Debug.Log("Do stuff");

        for (int i = 0; i < 100; i++)
        {

        }

        yield return null;
    }

    private IEnumerator test2()
    {
        yield return null;

        Debug.Log("Do stuff");

        for (int i = 0; i < 100; i++)
        {

        }
    }

    private IEnumerator test3()
    {
        yield return null;
        Debug.Log("Do stuff");

        for (int i = 0; i < 100; i++)
        {
            if(i == 10)
                yield return null;
        }
    }
}
