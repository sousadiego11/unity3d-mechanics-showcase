using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rotationSpeed = 400f;

    [Header("[Dependencies]")]
    [SerializeField] Camera cam;
    [SerializeField] Animator animator;
    [SerializeField] CharacterController characterController;

    [Header("[Ground Check]")]
    [SerializeField] float groundedRadius;
    [SerializeField] Vector3 groundedOffset;
    [SerializeField] LayerMask groundLayer;
    public bool isGrounded;

    Vector3 direction;

    void Update() {
        CheckGrounded();
        Move();
    }

    void Move() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float axisFactor = Mathf.Clamp01(Mathf.Abs(x) + Mathf.Abs(z));
        Vector3 axisDirection = new Vector3(x, 0, z).normalized;

        direction = cam.transform.rotation * axisDirection;
        direction.y = 0;

        if (axisFactor > 0) {
            characterController.Move(direction * movementSpeed * Time.deltaTime);

            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }

        animator.SetFloat("AxisOffset", axisFactor, 0.2f, Time.deltaTime);
    }

    void CheckGrounded() {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundedOffset), groundedRadius, groundLayer);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundedOffset), groundedRadius);
    }
}
