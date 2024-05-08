using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Vector3 targetPositionCache;

    void Start() {
        TargetPointer targetPointer = GameObject.FindGameObjectWithTag("Pointer").GetComponent<TargetPointer>();
        targetPositionCache = targetPointer.targetPosition;
    }

    void Update() {
        Debug.Log("Magnitude: " + targetPositionCache.magnitude);
        Debug.DrawRay(transform.position, targetPositionCache - transform.position, Color.magenta);
        transform.position = Vector3.MoveTowards(transform.position, targetPositionCache, Time.deltaTime * 5f);
    }
}
