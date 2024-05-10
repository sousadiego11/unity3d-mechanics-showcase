using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] Camera cam;

    Vector3 direction;

    void Update() {
        Move();
    }

    void Move() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveInput = new Vector3(x, 0, z).normalized;

        direction = cam.transform.rotation * moveInput;
        direction.y = 0;

        if (!Vector3.Equals(direction, Vector3.zero)) {
            transform.position += direction * movementSpeed * Time.deltaTime;

            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.Slerp(transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }
    }
}
