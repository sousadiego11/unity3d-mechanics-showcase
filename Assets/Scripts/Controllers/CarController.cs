using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : Mechanic
{
    [Header("[Movement]")]
    [SerializeField] float torqueForce;
    [SerializeField] float steerForce;
    [SerializeField] float maxSpeed;

    [Header("[Interaction]")]
    [SerializeField] bool steering;
    [SerializeField] bool accelerating;
    
    [Header("[Generated Dynamics]")]
    [SerializeField] private float speed;
    [SerializeField] private Vector3 movementAxis;

    void Update() {
        CheckInteractions();
        CheckSpeed();
        MoveCar();
    }

    void CheckInteractions() {
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");
        movementAxis = new Vector3(x, 0f, z).normalized;

        steering = movementAxis.z < 0f;
        accelerating = movementAxis.z > 0f;
    }

    void CheckSpeed() {
        if (accelerating) {
            float incrementalForce = Mathf.Abs(movementAxis.z) * Time.deltaTime * torqueForce;
            speed += incrementalForce;
        } else if (steering) {
            float decrementalForce = Mathf.Abs(movementAxis.z) * Time.deltaTime * steerForce;
            speed -= decrementalForce;
        }
    }

    void MoveCar() {
    }
}
