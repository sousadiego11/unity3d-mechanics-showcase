using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [SerializeField] LayerMask layerMaskTarget;
    [SerializeField] Camera cam;
    [HideInInspector] public Vector3 targetPosition;
    public bool isHiting = false;

    public void Raycast() {
        Vector3 mouseScreenPos = Input.mousePosition; // Position with x and z relative to the camera view
        Vector3 mouseScreenPosWithDistance = new(mouseScreenPos.x, mouseScreenPos.y, cam.farClipPlane); // Position with the distance set to the camera far
        Vector3 mouseWorldPoint = cam.ScreenToWorldPoint(mouseScreenPosWithDistance); // Final world x,y and z coords
        
        Debug.DrawRay(cam.transform.position, mouseWorldPoint, Color.green);

        if (Physics.Raycast(cam.transform.position, mouseWorldPoint, out RaycastHit hit, cam.farClipPlane, layerMaskTarget)) {
            targetPosition = hit.point;
            transform.position = hit.point;
            isHiting = true;
        } else {
            isHiting = false;
        }
    }
}