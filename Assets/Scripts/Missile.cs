using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Vector3 targetPositionCache;
    [SerializeField] GameObject effectToInstantiate;
    [SerializeField] float explosionPower;
    [SerializeField] float explosionRadius;

    void Start() {
        TargetPointer targetPointer = GameObject.FindGameObjectWithTag("Pointer").GetComponent<TargetPointer>();
        targetPositionCache = targetPointer.targetPosition;
    }

    void Update() {
        Debug.DrawRay(transform.position, targetPositionCache - transform.position, Color.magenta);
        Vector3 direction = (targetPositionCache - transform.position).normalized;
        transform.Translate(5f * Time.deltaTime * direction, Space.World);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Hittable")) {
            Instantiate(effectToInstantiate, transform.position, Quaternion.identity);
            foreach (Collider hitCollider in Physics.OverlapSphere(transform.position, 5)) {
                bool foundRb = hitCollider.TryGetComponent(out Rigidbody rb);
                if (foundRb) rb.AddExplosionForce(explosionPower, transform.position, explosionRadius, 3.0F);
            }
            Destroy(gameObject);
        }
    }
}
