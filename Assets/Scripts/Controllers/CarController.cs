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
    [SerializeField] LayerMask layerMask;
    [SerializeField] bool isGrounded;

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

    float SuspensionMaxDist(SpringTip springTip, Tire tire) {
        return springTip.maxDist + tire.mesh.bounds.size.y;
    }

    void HandlePhysics() {
        foreach (WheelAssembly WA in wheelAssemblies) {
            SpringTip springTip = WA.springTip;
            Tire tire = WA.tire;
            float rayDistance = SuspensionMaxDist(springTip, tire);

            Debug.DrawRay(springTip.transform.position, Vector3.down * rayDistance, Color.green);

            if (Physics.Raycast(springTip.transform.position, -springTip.transform.up, out RaycastHit hit, rayDistance, layerMask)) {
                isGrounded = true;
                HandleSuspensionPhysics(springTip, tire, hit);
                HandleSteerPhysics(tire, hit);
                HandleTorquePhysics(tire, hit);
            } else {
                isGrounded = false;
            }
        }
    }

    void HandleSuspensionPhysics(SpringTip springTip, Tire tire, RaycastHit hit) {
        Vector3 springVel = rb.GetPointVelocity(springTip.transform.position);

        float projectedVelocity = Vector3.Dot(springTip.transform.up, springVel);
        float springOffset = SuspensionMaxDist(springTip, tire) - hit.distance;
        float force = (springOffset * springTip.strength) - (projectedVelocity * springTip.damping);

        rb.AddForceAtPosition(springTip.transform.up * force, springTip.transform.position);
    }

    // Basic newton force law
    void HandleTorquePhysics(Tire tire, RaycastHit _) {
        if (Mathf.Abs(rb.velocity.magnitude) < maxTorque) {
            float accelerationInput = Input.GetAxis("Vertical");
            float accelerationFactor = accelerationInput * acceleration;
            float torque = rb.mass / 4 * accelerationFactor;

            rb.AddForceAtPosition(tire.transform.forward * torque, tire.transform.position);
        } else {
            rb.velocity = rb.velocity.normalized * maxTorque;
        }
    }

    void HandleSteerPhysics(Tire tire, RaycastHit _) {
        // Vector3 steerDir = tire.transform.right;
        // Vector3 tireVel = rb.GetPointVelocity(tire.transform.position);

        // float currentSteeringVel = Vector3.Dot(steerDir, tireVel);
        // float newSteeringVel = -currentSteeringVel * tire.grip;
        // float tireMass = rb.mass / wheelAssemblies.Count;
        // Vector3 force = steerDir * tireMass * newSteeringVel;

        // Debug.Log("currentSteeringVel: " + currentSteeringVel);
        // Debug.Log("newSteeringVel: " + newSteeringVel);
        // Debug.Log("tire.grip: " + tire.grip);
        // Debug.Log("force: " + force);

        // rb.AddForceAtPosition(force, tire.transform.position);

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
                    tire.transform.localRotation = Quaternion.Lerp(tire.transform.localRotation, Quaternion.Euler(tire.transform.localEulerAngles.x, deltaLeft, tire.transform.localEulerAngles.z), Time.deltaTime * 5);
                } else if (tire.position == Tire.Position.FR) {
                    tire.transform.localRotation = Quaternion.Lerp(tire.transform.localRotation, Quaternion.Euler(tire.transform.localEulerAngles.x, deltaRight, tire.transform.localEulerAngles.z), Time.deltaTime * 5);
                }
            }
        }
    }
}
