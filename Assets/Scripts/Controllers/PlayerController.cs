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
    [SerializeField] public bool isMoving;
    [SerializeField] public bool isRunning;
    [SerializeField] public bool isRecovering;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isJumping;
    [SerializeField] private float gravityForce;
    [SerializeField] private float velocity = 1f;

    Vector3 axisNormalizedDirection;

    void Update() {
        LockMovementOrInteract();

        CheckVerticalMovements();
        CheckGroundVelocity();
        
        HandleMovement();
        HandleAnimation();
    }

    PlayerMovement GetMovementStrategy() {
        if (isRunning) {
            return playerMovements.Find(m => m.identifier == PlayerMovement.NameEnum.Running);
        } else {
            return playerMovements.Find(m => m.identifier == PlayerMovement.NameEnum.Default);
        }
    }

    void HandleMovement() {
        Vector3 direction = cam.transform.rotation * axisNormalizedDirection;
        direction.y = 0f;

        Vector3 movementMotion = velocity * direction;
        movementMotion.y = gravityForce;
        characterController.Move(movementMotion * Time.deltaTime);

        if (isMoving) {
            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, GetMovementStrategy().rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }

    }

    void HandleAnimation() {
        animator.SetFloat("Velocity", velocity);

        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isGrounded", isGrounded);
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
        bool running = Input.GetKey(KeyCode.LeftShift);

        Vector3 newDir = new(x, 0, z);
        
        axisNormalizedDirection = newDir.normalized;
        isMoving = Mathf.Clamp01(Mathf.Abs(newDir.x) + Mathf.Abs(newDir.z)) > 0;
        isRunning = running;
    }

    void CheckGroundVelocity() {
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

    void CheckVerticalMovements() {
        PlayerMovement movementSTR = GetMovementStrategy();

        if ((Input.GetKeyDown(KeyCode.Space) && isGrounded) || isJumping) {
            gravityForce = Mathf.Lerp(gravityForce, Mathf.Abs(Physics.gravity.y) * 3, movementSTR.jumpSpeed * Time.deltaTime);
            isJumping = true;
            isFalling = false;
        } else if (isGrounded) {
            gravityForce = -1;
            isJumping = false;
            isFalling = false;
        } else {
            gravityForce = Mathf.Lerp(gravityForce, Physics.gravity.y, movementSTR.fallSpeed * Time.deltaTime);
        }

        isGrounded = !isJumping && Physics.CheckSphere(transform.TransformPoint(groundedOffset), groundedRadius, groundLayer);

        if (gravityForce >= Mathf.Abs(Physics.gravity.y) * 2.8) {
            isFalling = true;
            isJumping = false;
            isGrounded = false;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundedOffset), groundedRadius);
    }

}
