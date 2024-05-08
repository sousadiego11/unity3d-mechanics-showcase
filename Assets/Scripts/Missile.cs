using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Vector3 direction;

    void Start() {
        TargetPointer targetPointer = GameObject.FindGameObjectWithTag("Pointer").GetComponent<TargetPointer>();
        direction = targetPointer.targetPosition - transform.position;
    }

    void Update() {
        // Debug.Log("Magnitude: " + direction.magnitude);
        // Debug.DrawRay(transform.position, direction, Color.magenta);
        // transform.Translate(direction.normalized * Time.deltaTime);
    }
}
