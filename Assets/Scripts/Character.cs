using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rotationSpeed = 400f;
    [SerializeField] [Range(1f, 2f)] float fallSpeed = 1f;

    [Header("[Dependencies]")]
    [SerializeField] Camera cam;
    [SerializeField] Animator animator;
    [SerializeField] CharacterController characterController;

    [Header("[Ground Check]")]
    [SerializeField] float groundedRadius;
    [SerializeField] Vector3 groundedOffset;
    [SerializeField] LayerMask groundLayer;

    public bool isGrounded;
    Vector3 axisNormalizedDirection;
    float fallAccumulatedForce;
    float axisAbsDisplacement;

    void Update() {
        CheckInputs();
        CheckGrounded();
        CheckFallingSpeed();
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement() {
        Vector3 direction = cam.transform.rotation * axisNormalizedDirection;
        direction.y = 0f;

        Vector3 movementMotion = movementSpeed * direction;
        movementMotion.y = fallAccumulatedForce;
        characterController.Move(movementMotion * Time.deltaTime);

        if (Mathf.Clamp01(axisAbsDisplacement) > 0) {
            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }

    }

    void HandleAnimation() {
        animator.SetBool("isFalling", !isGrounded);
        animator.SetFloat("AxisOffset", Mathf.Clamp01(axisAbsDisplacement), 0.2f, Time.deltaTime);
    }

    void CheckInputs() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        axisAbsDisplacement = Mathf.Abs(x) + Mathf.Abs(z);
        axisNormalizedDirection = new Vector3(x, 0, z).normalized;

    }

    void CheckFallingSpeed() {
        if (isGrounded) {
            fallAccumulatedForce = -1f;
        } else {
            fallAccumulatedForce += Physics.gravity.y * fallSpeed * Time.deltaTime;
        }
    }

    void CheckGrounded() {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundedOffset), groundedRadius, groundLayer);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundedOffset), groundedRadius);
    }
}
