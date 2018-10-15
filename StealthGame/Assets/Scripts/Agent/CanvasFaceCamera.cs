using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    private Camera m_mainCamera = null;
    private void Start()
    {
        m_mainCamera = Camera.main;
    }
    private void Update ()
    {
        transform.LookAt(m_mainCamera.transform);
	}
}
