using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimplePointer : MonoBehaviour
{
    [SerializeField] LayerMask layerMaskTarget;
    [SerializeField] Camera cam;
    [SerializeField] GameObject placeholder;
    [HideInInspector] public Vector3 hitPos;

    public void Raycast() {
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 sourceWorld = cam.ScreenToWorldPoint(new Vector3(screenCenter.x, screenCenter.y, cam.nearClipPlane));
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenCenter.x, screenCenter.y, cam.farClipPlane));
        Vector3 direction = (worldPos - sourceWorld).normalized;

        if (Physics.Raycast(sourceWorld, direction, out RaycastHit hit, cam.farClipPlane, layerMaskTarget)) {
            hitPos = hit.point;
            placeholder.transform.position = hit.point;
            Debug.DrawLine(sourceWorld, hit.point, Color.red);
        }
    }
}