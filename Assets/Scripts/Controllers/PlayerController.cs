using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerController : Mechanic
{
    [Header("[Movement]")]
    [SerializeField] private List<PlayerMovement> playerMovements;

    [Header("[Dependencies]")]
    [SerializeField] private  CameraFollowController cam;
    [SerializeField] private  Animator animator;
    [SerializeField] private  CharacterController characterController;

    [Header("[Ground Check]")]
    [SerializeField] private  float groundedRadius;
    [SerializeField] private  Vector3 groundedOffset;
    [SerializeField] private  LayerMask groundLayer;


    [Header("[State]")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFalling;
    [SerializeField] private float fallingMagnitude;
    [SerializeField] private float velocity = 1f;
    public bool isMoving;
    public bool isRunning;
    public bool isAiming;
    public bool isRecovering;

    Vector3 axisNormalizedDirection;
    Vector3 axisDirection;

    void Update() {
        LockMovementOrInteract();

        CheckGrounded();
        CheckFallingSpeed();
        CheckVelocity();
        
        HandleMovement();
        HandleAnimation();

    }

    PlayerMovement GetMovementStrategy() {
        if (isRunning && !isAiming) {
            return playerMovements.Find(m => m.identifier == PlayerMovement.NameEnum.Running);
        } else if (isAiming) {
            return playerMovements.Find(m => m.identifier == PlayerMovement.NameEnum.Aiming);
        } else {
            return playerMovements.Find(m => m.identifier == PlayerMovement.NameEnum.Default);
        }
    }

    void HandleMovement() {
        Vector3 direction = cam.transform.rotation * axisNormalizedDirection;
        direction.y = 0f;

        Vector3 movementMotion = velocity * direction;
        movementMotion.y = fallingMagnitude;
        characterController.Move(movementMotion * Time.deltaTime);

        if (isMoving | isAiming) {
            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, GetMovementStrategy().rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }

    }

    void HandleAnimation() {
        animator.SetFloat("Velocity", velocity);

        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isAiming", isAiming);

        animator.SetFloat("XAxis", axisDirection.x * velocity);
        animator.SetFloat("ZAxis", axisDirection.z * velocity);
        isRecovering = animator.GetCurrentAnimatorStateInfo(0).IsName("Falling To Landing");
    }

    void LockMovementOrInteract() {
        if (Locked()) {
            velocity = 0f;
        } else {
            CheckInteractions();
        }
    }

    void CheckInteractions() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool aiming = Input.GetMouseButton(1);
        bool running = Input.GetKey(KeyCode.LeftShift);

        Vector3 newDir = new(x, 0, z);
        
        axisDirection = newDir;
        axisNormalizedDirection = newDir.normalized;
        isMoving = Mathf.Clamp01(Mathf.Abs(newDir.x) + Mathf.Abs(newDir.z)) > 0;
        isAiming = aiming;
        isRunning = running;

        UIController.Instance.PlayerUI(isAiming);
    }

    void CheckVelocity() {
        PlayerMovement movementSTR = GetMovementStrategy();
        float speedToAchieve;
        float multiplier = isMoving ? movementSTR.acceleration : movementSTR.deceleration;
        
        if (isFalling || !isMoving || isRecovering) {
            speedToAchieve = 0f;
        } else {
            speedToAchieve = movementSTR.maximumSpeed;
        }

        velocity = Mathf.Lerp(velocity, speedToAchieve, multiplier * Time.deltaTime);
    }

    void CheckFallingSpeed() {
        PlayerMovement movementSTR = GetMovementStrategy();

        if (isGrounded) {
            fallingMagnitude = -1;
        } else {
            fallingMagnitude = Mathf.Lerp(fallingMagnitude, Physics.gravity.y, movementSTR.fallSpeed * Time.deltaTime);
        }

        isFalling = Mathf.Abs(fallingMagnitude) > Mathf.Abs(Physics.gravity.y / 3);
    }

    void CheckGrounded() {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundedOffset), groundedRadius, groundLayer);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundedOffset), groundedRadius);
    }

}
