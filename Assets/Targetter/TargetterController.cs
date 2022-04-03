using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetterController : MonoBehaviour
{
    Camera mainCamera;

    Rect guiRect = new Rect(0f, 0f, 400f, 400f);
    GUIStyle guiStyle = new GUIStyle();

    void Awake()
    {
        mainCamera = Camera.main;

        guiStyle.fontSize = 30;
    }

    void Update()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseWorldPos.z = this.transform.position.z;

        transform.LookAt(mouseWorldPos);
    }

    public float GetAngle()
    {
        return Vector3.SignedAngle((transform.forward).normalized, Vector3.up, Vector3.forward) - 90f;
    }
}
