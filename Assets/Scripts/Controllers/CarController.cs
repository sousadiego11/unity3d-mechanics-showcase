using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : Mechanic
{
    [Header("[Car Parts]")]
    [SerializeField] List<WheelAssembly> wheelAssemblies;

    [Header("[Car Movement]")]
    [SerializeField] float acceleration;
    [SerializeField] float maxTorque;

    private Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        HandleForces();
    }

    void HandleForces() {
        foreach (WheelAssembly WA in wheelAssemblies) {
            Spring spring = WA.spring;
            Tire tire = WA.tire;

            Debug.DrawRay(spring.transform.position, Vector3.down * spring.restDist, Color.green);

            HandleTireRotation(tire);
            if (Physics.Raycast(spring.transform.position, -spring.transform.up, out RaycastHit hit, spring.restDist)) {
                HandleSuspensions(spring, hit);
                HandleForwardTorque(tire, hit);
            }
        }
    }

    void HandleSuspensions(Spring spring, RaycastHit hit) {
        Vector3 tireVel = rb.GetPointVelocity(spring.transform.position);

        float projectedVelocity = Vector3.Dot(spring.transform.up, tireVel);
        float springOffset = spring.restDist - hit.distance;
        float force = (springOffset * spring.strength) - (projectedVelocity * spring.damping);

        rb.AddForceAtPosition(spring.transform.up * force, spring.transform.position);
    }

    // Basic newton force law
    void HandleForwardTorque(Tire tire, RaycastHit _) {
        if (Mathf.Abs(rb.velocity.magnitude) < maxTorque) {
            float accelerationInput = Input.GetAxis("Vertical");
            float accelerationFactor = accelerationInput * acceleration;
            float torque = rb.mass * accelerationFactor;

            rb.AddForceAtPosition(tire.transform.forward * torque, tire.transform.position);
        } else {
            rb.velocity = rb.velocity.normalized * maxTorque;
        }
    }

    void HandleTireRotation(Tire tire) {
        
    }
}
