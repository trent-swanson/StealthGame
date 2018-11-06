using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    private CameraController m_cameraController = null;
    private void Start()
    {
        m_cameraController = GameObject.FindGameObjectWithTag("CamPivot").GetComponent<CameraController>();

#if UNITY_EDITOR
        if (m_cameraController != null)
            Debug.Log("Canvas is unable to find camera Pivot");
#endif
    }

    private void Update ()
    {
        transform.rotation = m_cameraController.transform.rotation;
    }
}
