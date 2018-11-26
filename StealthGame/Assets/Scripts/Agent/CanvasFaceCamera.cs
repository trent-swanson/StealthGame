using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    private CameraController m_cameraController = null;

    //--------------------------------------------------------------------------------------
    // Initialisation
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        m_cameraController = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();

#if UNITY_EDITOR
        if (m_cameraController == null)
            Debug.Log("Canvas is unable to find camera Pivot");
#endif
    }

    //--------------------------------------------------------------------------------------
    // Rotate canvas to face the cameras pivot 
    //--------------------------------------------------------------------------------------
    private void Update ()
    {
        transform.rotation = m_cameraController.m_cameraPivot.transform.rotation;
    }
}
