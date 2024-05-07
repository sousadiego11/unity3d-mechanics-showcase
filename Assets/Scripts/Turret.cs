using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] GameObject rayPoint;
    [SerializeField] GameObject turretHead;
    [SerializeField] GameObject turretBase;
    [SerializeField] LayerMask layerMaskTarget;
    [SerializeField] Camera cam;

    void Update() {
        Aim();
    }

    void Aim() {
        Vector3 mouseScreenPos = Input.mousePosition; // Position with x and z relative to the camera view
        Vector3 mouseScreenPosWithDistance = new(mouseScreenPos.x, mouseScreenPos.y, cam.farClipPlane); // Position with the distance set to the camera far
        Vector3 mouseWorldPoint = cam.ScreenToWorldPoint(mouseScreenPosWithDistance); // Final world x,y and z coords
        
        Debug.DrawRay(cam.transform.position, mouseWorldPoint, Color.green);

        if (Physics.Raycast(cam.transform.position, mouseWorldPoint, out RaycastHit hit, cam.farClipPlane, layerMaskTarget)) {
            Vector3 resultedEulerTarget = turretBase.transform.position - hit.point;

            float verticalAngle = Mathf.Asin(resultedEulerTarget.y / resultedEulerTarget.magnitude) * Mathf.Rad2Deg;
            float horizontalAngle = Mathf.Atan2(-resultedEulerTarget.x, -resultedEulerTarget.z) * Mathf.Rad2Deg;

            if (horizontalAngle < 0f) horizontalAngle += 360f;

            Quaternion headRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
            Quaternion baseRotation = Quaternion.Euler(0f, horizontalAngle, 0f);

            turretHead.transform.rotation = headRotation;
            turretBase.transform.rotation = baseRotation;
            rayPoint.transform.position = hit.point;
        }
    }
}