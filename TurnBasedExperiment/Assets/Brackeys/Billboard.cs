using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        // Find the camera with the "cam" tag
        GameObject cameraObj = GameObject.FindGameObjectWithTag("Cam");

        // If camera is found, get its transform
        if (cameraObj != null)
        {
            cam = cameraObj.transform;
        }
        else
        {
            Debug.LogError("No camera tagged as 'cam' found!");
        }
    }

    void LateUpdate()
    {
        // If camera is found, make the object face it
        if (cam != null)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}