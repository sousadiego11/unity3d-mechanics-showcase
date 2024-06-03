using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : Mechanic
{
    [Header("[Car Parts]")]
    [SerializeField] List<WheelAssembly> wheelAssemblies;

    [Header("[Car Movement]")]
    [SerializeField] float acceleration;
    [SerializeField] [Tooltip("Maximum speed in Km/H")] float maxSpeed;
    [SerializeField] [Tooltip("Current speed in Km/H")] float currentSpeed;
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

    float TireMass() {
        return rb.mass * 0.05f;
    }

    float TireRadius(Tire tire) {
        return Mathf.Max(tire.mesh.bounds.size.x, tire.mesh.bounds.size.z) / 2f;
    }

    float CarSpeedKMH() {
        return rb.velocity.magnitude * 3.6f;
    }

    void HandlePhysics() {
        foreach (WheelAssembly WA in wheelAssemblies) {
            SpringTip springTip = WA.springTip;
            Tire tire = WA.tire;
            float rayDistance = springTip.maxDist + TireRadius(tire);

            Debug.DrawRay(springTip.transform.position, Vector3.down * rayDistance, Color.green);

            if (Physics.Raycast(springTip.transform.position, -springTip.transform.up, out RaycastHit hit, rayDistance, layerMask)) {
                isGrounded = true;
                HandleSuspensionPhysics(springTip, tire, hit);
                HandleTorquePhysics(tire, hit);
                HandleSteerPhysics(tire, hit);
            } else {
                isGrounded = false;
            }
        }
    }

    void HandleSuspensionPhysics(SpringTip springTip, Tire tire, RaycastHit hit) {
        Vector3 springVel = rb.GetPointVelocity(springTip.transform.position);

        float projectedVelocity = Vector3.Dot(springTip.transform.up, springVel);
        float springOffset = springTip.restDist - (hit.distance - TireRadius(tire));
        float force = (springOffset * springTip.strength) - (projectedVelocity * springTip.damping);

        rb.AddForceAtPosition(springTip.transform.up * force, springTip.transform.position);
        Debug.DrawRay(springTip.transform.position, springTip.transform.up * force * 0.005f, Color.green);
    }

    // Basic newton force law
    void HandleTorquePhysics(Tire tire, RaycastHit _) {
        if (Mathf.Abs(CarSpeedKMH()) < maxSpeed) {
            float accelerationInput = Input.GetAxis("Vertical");
            float accelerationFactor = accelerationInput * acceleration;
            float torque = rb.mass / wheelAssemblies.Count * accelerationFactor;

            rb.AddForceAtPosition(tire.transform.forward * torque, tire.transform.position);
            Debug.DrawRay(tire.transform.position, tire.transform.forward * torque * 0.02f, Color.blue);
            currentSpeed = CarSpeedKMH();
        }
    }

    void HandleSteerPhysics(Tire tire, RaycastHit _) {
        Vector3 steerDirection = tire.transform.right;
        Vector3 tireVelocity = rb.GetPointVelocity(tire.transform.position);

        float steeringVelocity = Vector3.Dot(steerDirection, tireVelocity);
        float desiredChangeVelocity = -steeringVelocity * tire.grip / Time.fixedDeltaTime;

        Vector3 force = steerDirection * TireMass() * desiredChangeVelocity;

        rb.AddForceAtPosition(force, tire.transform.position);
        Debug.DrawRay(tire.transform.position, steerDirection * force.magnitude * 0.02f, Color.red);
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
