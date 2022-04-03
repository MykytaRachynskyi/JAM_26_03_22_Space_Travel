using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] float fovStep;
    [SerializeField] float fovMin;
    [SerializeField] float fovMax;

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y > 0f)
        {
            Debug.Log(Input.mouseScrollDelta.ToString());
            mainCam.fieldOfView -= fovStep;

            mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView, fovMin, fovMax);
        }
        else if(Input.mouseScrollDelta.y < 0f)
        {
            mainCam.fieldOfView += fovStep;

            mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView, fovMin, fovMax);
        }
    }
}
