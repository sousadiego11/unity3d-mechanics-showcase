using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : Mechanic
{
    [SerializeField] List<Transform> tires;
    [SerializeField] float springStrength;
    [SerializeField] float springDamping;
    [SerializeField] float springRestDistance;

    private Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        foreach (Transform tire in tires) {

            Vector3 springDirection = tire.up;
            Debug.DrawRay(tire.position, Vector3.down * springRestDistance, Color.green);

            if (Physics.Raycast(tire.position, -springDirection, out RaycastHit hit)) {
                Vector3 tireVel = rb.GetPointVelocity(tire.position);

                float projectedVelocity = Vector3.Dot(springDirection, tireVel);
                float springOffset = springRestDistance - hit.distance;
                float force = (springOffset * springStrength) - (projectedVelocity * springDamping);

                rb.AddForceAtPosition(springDirection * force, tire.position);

                Debug.Log("springOffset: " + springOffset);
            }
        }
    }
}
