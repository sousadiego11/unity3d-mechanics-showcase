using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField] float maximumSpeed = 5f;
    [SerializeField] [Range(1f, 10f)] float acceleration = 1f;
    [SerializeField] [Range(1f, 10f)] float deceleration = 1f;
    [SerializeField] float rotationSpeed = 400f;
    [SerializeField] [Range(1f, 5f)] float fallSpeed = 1f;

    [Header("[Dependencies]")]
    [SerializeField] Camera cam;
    [SerializeField] Animator animator;
    [SerializeField] CharacterController characterController;

    [Header("[Ground Check]")]
    [SerializeField] float groundedRadius;
    [SerializeField] Vector3 groundedOffset;
    [SerializeField] LayerMask groundLayer;


    [Header("[State]")]
    public bool isGrounded;
    public bool isMoving;
    public bool isFalling;
    public float fallingMagnitude;
    public float velocity = 1f;

    Vector3 axisNormalizedDirection;
    float axisAbsDisplacement;

    void Update() {
        CheckMovementInteraction();
        CheckGrounded();
        CheckFallingSpeed();
        CheckCurrentSpeed();
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement() {
        Vector3 direction = cam.transform.rotation * axisNormalizedDirection;
        direction.y = 0f;

        Vector3 movementMotion = velocity * direction;
        movementMotion.y = fallingMagnitude;
        characterController.Move(movementMotion * Time.deltaTime);

        if (isMoving) {
            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }

    }

    void HandleAnimation() {
        animator.SetBool("isFalling", !isGrounded);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("Velocity", velocity);
    }

    void CheckMovementInteraction() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 newDir = new Vector3(x, 0, z).normalized;
        axisAbsDisplacement = Mathf.Abs(newDir.x) + Mathf.Abs(newDir.z);
        isMoving = Mathf.Clamp01(axisAbsDisplacement) > 0;
        
        CheckDirectionCanceling(axisNormalizedDirection, newDir, isMoving);

        axisNormalizedDirection = newDir;
    }

    void CheckCurrentSpeed() {
        float newSpeed;
        
        if (isFalling) {
            newSpeed = velocity - Mathf.Abs(fallingMagnitude) * fallSpeed * Time.deltaTime;
        } else if (isMoving) {
            newSpeed = velocity + acceleration * Time.deltaTime;
        } else {
            newSpeed = velocity - deceleration * Time.deltaTime;
        }
        velocity = Mathf.Clamp(newSpeed, 0, maximumSpeed);
    }

    void CheckFallingSpeed() {
        if (isGrounded) {
            fallingMagnitude = -1f;
        } else {
            fallingMagnitude += Physics.gravity.y * fallSpeed * Time.deltaTime;
        }

        isFalling = Mathf.Abs(fallingMagnitude) > 1;
    }

    void CheckGrounded() {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundedOffset), groundedRadius, groundLayer);
    }

    void CheckDirectionCanceling(Vector3 previousDir, Vector3 newDir, bool isMoving) {
        Vector3 roundedPrev = Vector3Int.RoundToInt(previousDir);
        Vector3 roundedNew = Vector3Int.RoundToInt(newDir);
        
        if (Vector3.Dot(roundedPrev, roundedNew) == -1 && isMoving) {
            velocity = 0f;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundedOffset), groundedRadius);
    }

}
