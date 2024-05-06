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

    void Update() {
        Aim();
    }

    void Aim() {
        Vector3 mouseScreenPos = Input.mousePosition; // Position with x and z relative to the camera view
        Vector3 mouseScreenPosWithDistance = new(mouseScreenPos.x, mouseScreenPos.y, Camera.main.farClipPlane); // Position with the distance set to the camera far
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(mouseScreenPosWithDistance); // Final world x,y and z coords
        
        Debug.DrawRay(transform.position, mouseWorldPoint, Color.green);

        if (Physics.Raycast(transform.position, mouseWorldPoint, out RaycastHit hit, Camera.main.farClipPlane, layerMaskTarget)) {
            Vector3 resultedEulerTarget = transform.position - hit.point;

            float verticalAngle = Mathf.Atan2(resultedEulerTarget.y, resultedEulerTarget.z) * Mathf.Rad2Deg;
            float horizontalAngle = Mathf.Atan2(resultedEulerTarget.x, resultedEulerTarget.z) * Mathf.Rad2Deg;

            Quaternion headRotation = Quaternion.Euler(verticalAngle, horizontalAngle, turretHead.transform.position.z);
            Quaternion baseRotation = Quaternion.Euler(turretBase.transform.position.x, horizontalAngle, turretBase.transform.position.z);

            turretHead.transform.rotation = headRotation;
            turretBase.transform.rotation = baseRotation;
            rayPoint.transform.position = hit.point;
        }
    }
}