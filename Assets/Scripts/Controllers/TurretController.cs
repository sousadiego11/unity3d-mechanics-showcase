using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TurretController : Mechanic
{
    [Header("[Parts]")]
    [SerializeField] GameObject turretHead;
    [SerializeField] GameObject turretBase;
    [SerializeField] GameObject missilePlaceholder;
    [Header("[Depedencies]")]
    [SerializeField] MissileController missilePrefab;
    [SerializeField] MousePointer mousePointer;
    [SerializeField] Animator animator;
    [Header("[Fire]")]
    [SerializeField] float reloadTime;
    bool reloading = false;
    LineRenderer lineRenderer;

    void Start() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.04f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;   
    }
    void Update() {
        if (Locked()) return;
        mousePointer.Raycast();
        Move();
        HandleShoot();
    }
 
    // Not using LookRotation was intended since the goal was to understand how to know the Vertical and Horizontal angles
    void Move() {
        if (mousePointer.isHiting) {
            Vector3 direction = mousePointer.targetPosition - turretHead.transform.position;
            Debug.DrawRay(missilePlaceholder.transform.position, direction, Color.cyan);
            lineRenderer.SetPosition(0, missilePlaceholder.transform.position);
            lineRenderer.SetPosition(1, mousePointer.targetPosition);

            float verticalAngle = Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg * -1;
            
            float horizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            horizontalAngle += horizontalAngle < 0f ? 360f : 0; // Gets the angle normalized to 360

            Quaternion headRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
            turretHead.transform.rotation = headRotation;
            
            Quaternion baseRotation = Quaternion.Euler(0f, horizontalAngle, 0f);
            turretBase.transform.rotation = baseRotation;

        }
    }

    void HandleShoot() {
        if (Input.GetMouseButtonDown(0) && mousePointer.isHiting && !reloading) {
            Shoot();
        }
    }

    void Shoot() {
        SoundBoard.Instance.PlayOne(Audio.AudioEnum.TurretShootSFX, 1f);
        MissileController missile = Instantiate(missilePrefab, missilePlaceholder.transform.position, missilePlaceholder.transform.rotation);
        missile.Init(mousePointer.targetPosition);
        StartCoroutine(ReloadTimer());
    }

    IEnumerator ReloadTimer() {
        reloading = true;
        animator.Play("TurretShoot");
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }
}