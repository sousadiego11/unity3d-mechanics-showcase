using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenPointer : MonoBehaviour
{
    [SerializeField] LayerMask layerMaskTarget;
    [SerializeField] Camera cam;
    [HideInInspector] public Vector3 hitPos;

    public void Raycast() {
        Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);
        Ray ray = cam.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, cam.farClipPlane, layerMaskTarget)) {
            hitPos = hit.point;
        }
    }
}