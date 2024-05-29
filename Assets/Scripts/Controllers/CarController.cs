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
    [SerializeField] float wheelBase;
    [SerializeField] float trackWidth;

    private Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        HandleSteering();
    }

    void FixedUpdate() {
        HandlePhysics();
    }

    void HandlePhysics() {
        foreach (WheelAssembly WA in wheelAssemblies) {
            Spring spring = WA.spring;
            Tire tire = WA.tire;
            Debug.DrawRay(spring.transform.position, Vector3.down * spring.restDist, Color.green);

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

    // Kinematics Ackerman Geometry
    void HandleSteering() {
        foreach (WheelAssembly WA in wheelAssemblies) {
            Tire tire = WA.tire;
            if (tire.position == Tire.Position.FL || tire.position == Tire.Position.FR) {
                float deltaSin = Input.GetAxis("Horizontal");
                float deltaAck = deltaSin * tire.maxSteerAngle * Mathf.Deg2Rad;
                float tanDeltaAck = Mathf.Tan(deltaAck);

                float deltaRight = Mathf.Atan( wheelBase * tanDeltaAck / (wheelBase - 0.5f * trackWidth * tanDeltaAck) ) * Mathf.Rad2Deg;
                float deltaLeft = Mathf.Atan( wheelBase * tanDeltaAck / (wheelBase + 0.5f * trackWidth * tanDeltaAck) ) * Mathf.Rad2Deg;

                if (tire.position == Tire.Position.FL) {
                    Quaternion newRotation = Quaternion.Euler(tire.transform.localEulerAngles.x, deltaLeft, tire.transform.localEulerAngles.z);
                    tire.transform.localRotation = newRotation;
                } else if (tire.position == Tire.Position.FR) {
                    Quaternion newRotation = Quaternion.Euler(tire.transform.localEulerAngles.x, deltaRight, tire.transform.localEulerAngles.z);
                    tire.transform.localRotation = newRotation;
                }
            }
        }
    }
}
