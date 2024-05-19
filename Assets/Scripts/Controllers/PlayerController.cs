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

    void Update() {
        LockMovementOrInteract();

        CheckGrounded();
        CheckFallingSpeed();
        CheckCurrentSpeed();
        
        HandleMovement();
        HandleAnimation();

    }

    PlayerMovement GetMovementStrategy() {
        if (isRunning) {
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

        if (isMoving) {
            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, GetMovementStrategy().rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }

    }

    void HandleAnimation() {
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("Velocity", velocity);
        isRecovering = animator.GetCurrentAnimatorStateInfo(0).IsName("Falling To Landing");
    }

    void LockMovementOrInteract() {
        if (Locked()) {
            velocity = 0f;
        } else if (isRecovering){
            velocity = 1f;
        } else {
            CheckInteractions();
        }
    }

    void CheckInteractions() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool aiming = Input.GetMouseButton(1);
        bool running = Input.GetKey(KeyCode.LeftShift);

        Vector3 newDir = new Vector3(x, 0, z).normalized;
        
        axisNormalizedDirection = newDir;
        isMoving = Mathf.Clamp01(Mathf.Abs(newDir.x) + Mathf.Abs(newDir.z)) > 0;
        isAiming = aiming;
        isRunning = running;
    }

    void CheckCurrentSpeed() {
        float newSpeed;
        
        if (isFalling) {
            newSpeed = velocity - Mathf.Abs(fallingMagnitude) * GetMovementStrategy().fallSpeed * Time.deltaTime;
        } else if (isMoving) {
            newSpeed = velocity + GetMovementStrategy().acceleration * Time.deltaTime;
        } else {
            newSpeed = velocity - GetMovementStrategy().deceleration * Time.deltaTime;
        }
        velocity = Mathf.Clamp(newSpeed, 0, GetMovementStrategy().maximumSpeed);
    }

    void CheckFallingSpeed() {
        if (isGrounded) {
            fallingMagnitude = -1f;
        } else {
            fallingMagnitude += Physics.gravity.y * GetMovementStrategy().fallSpeed * Time.deltaTime;
        }

        isFalling = Mathf.Abs(fallingMagnitude) > GetMovementStrategy().fallSpeed * 2;
    }

    void CheckGrounded() {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundedOffset), groundedRadius, groundLayer);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundedOffset), groundedRadius);
    }

}
