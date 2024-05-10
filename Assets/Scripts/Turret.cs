using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] GameObject turretHead;
    [SerializeField] GameObject turretBase;
    [SerializeField] GameObject missilePlaceholder;
    [SerializeField] Missile missilePrefab;
    [SerializeField] TargetPointer targetPointer;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float reloadTime;
    bool reloading = false;

    void Update() {
        Move();
        HandleShoot();
    }
 
    // Not using LookRotation was intended since the goal was to understand how to know the Vertical and Horizontal angles
    void Move() {
        if (targetPointer.isHiting) {
            Vector3 direction = targetPointer.targetPosition - turretHead.transform.position;
            Debug.DrawRay(missilePlaceholder.transform.position, direction, Color.cyan);

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
        if (Input.GetMouseButtonDown(0) && targetPointer.isHiting && !reloading) {
            Shoot();
        }
    }

    void Shoot() {
        audioSource.Play();
        Missile missile = Instantiate(missilePrefab, missilePlaceholder.transform.position, missilePlaceholder.transform.rotation);
        missile.Init(targetPointer);
        StartCoroutine(ReloadTimer());
    }

    IEnumerator ReloadTimer() {
        reloading = true;
        animator.Play("TurretShoot");
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }
}