using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : Mechanic
{
    [Header("Suspension")]
    [SerializeField] float springStrength;
    [SerializeField] float springDamping;
    [SerializeField] float springRestDistance;
    [SerializeField] List<Transform> tires;

    [Header("Movement")]
    [SerializeField] float acceleration;
    [SerializeField] float maxVelocity;

    private Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        HandleForces();
    }

    void HandleForces() {
        foreach (Transform tire in tires) {
            Debug.DrawRay(tire.position, Vector3.down * springRestDistance, Color.green);

            if (Physics.Raycast(tire.position, -tire.up, out RaycastHit hit, springRestDistance)) {
                HandleSuspensions(tire, hit);
                HandleForwardTorque(tire, hit);
            }
        }
    }

    void HandleSuspensions(Transform tire, RaycastHit hit) {
        Vector3 tireVel = rb.GetPointVelocity(tire.position);

        float projectedVelocity = Vector3.Dot(tire.up, tireVel);
        float springOffset = springRestDistance - hit.distance;
        float force = (springOffset * springStrength) - (projectedVelocity * springDamping);

        rb.AddForceAtPosition(tire.up * force, tire.position);
    }

    // Basic newton force law
    void HandleForwardTorque(Transform tire, RaycastHit _) {
        if (Mathf.Abs(rb.velocity.magnitude) < maxVelocity) {
            float accelerationInput = Input.GetAxis("Vertical");
            float accelerationFactor = accelerationInput * acceleration;
            float torque = rb.mass * accelerationFactor;

            rb.AddForceAtPosition(tire.forward * torque, tire.position);
        } else {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }
}
