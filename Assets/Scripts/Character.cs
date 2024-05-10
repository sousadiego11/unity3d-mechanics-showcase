using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] Rigidbody rb;

    Vector3 direction;

    void Update() {
        BuildDirection();
        RotateCharacter();
        MoveCharacter();
    }

    void BuildDirection() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        direction = new Vector3(x, 0, z).normalized;
    }

    void MoveCharacter() {
        if (!Vector3.Equals(direction, Vector3.zero)) {
            rb.velocity = movementSpeed * direction;
        }
    }

    void RotateCharacter() {
        if (!Vector3.Equals(direction, Vector3.zero)) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion playerRotationOffset = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = playerRotationOffset;
        }
    }
}
