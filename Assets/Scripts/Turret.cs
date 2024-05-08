using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] GameObject turretHead;
    [SerializeField] GameObject turretBase;
    [SerializeField] GameObject projectileToInstantiate;
    [SerializeField] GameObject missilePlaceholder;
    [SerializeField] TargetPointer targetPointer;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float reloadTime;
    bool reloading = false;

    void Update() {
        MoveHead();
        MoveBase();
        Shoot();
    }

    void MoveHead() {
        if (targetPointer.isHiting) {
            Vector3 direction = targetPointer.targetPosition - turretHead.transform.position;
            Debug.DrawRay(missilePlaceholder.transform.position, direction, Color.cyan);

            float verticalAngle = Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg * -1;
            float horizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Gets the angle normalized to 360
            if (horizontalAngle < 0f) horizontalAngle += 360f;

            Quaternion headRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
            turretHead.transform.rotation = headRotation;
        }
    }

    void MoveBase() {
        if (targetPointer.isHiting) {
            Vector3 direction = targetPointer.targetPosition - turretBase.transform.position;
            float horizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Gets the angle normalized to 360
            if (horizontalAngle < 0f) horizontalAngle += 360f;

            Quaternion baseRotation = Quaternion.Euler(0f, horizontalAngle, 0f);
            turretBase.transform.rotation = baseRotation;
        }
    }

    void Shoot() {
        if (Input.GetMouseButtonDown(0) && targetPointer.isHiting && !reloading) {
            audioSource.Play();
            Instantiate(projectileToInstantiate, missilePlaceholder.transform.position, missilePlaceholder.transform.rotation);
            StartCoroutine(ReloadTimer());
        }
    }

    IEnumerator ReloadTimer() {
        reloading = true;
        animator.SetBool("isShooting", true);
        yield return new WaitForSeconds(reloadTime);
        animator.SetBool("isShooting", false);
        reloading = false;
    }
}