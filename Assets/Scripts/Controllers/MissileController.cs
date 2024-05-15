using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    Vector3 targetPositionCache;
    
    [Header("[FX]")]
    [SerializeField] GameObject vfx;
    [Header("[Explosion]")]
    [SerializeField] float explosionPower;
    [SerializeField] float explosionRadius;
    [Header("[Speed]")]
    [SerializeField] float travelSpeed;
    [SerializeField] float spinSpeed;

    public void Init(Vector3 targetPosition) {
        targetPositionCache = targetPosition;
        SoundBoard.Instance.PlayOne(Audio.AudioEnum.MissileMoveSFX, 0.2f);
    }

    void Update() {
        Debug.DrawRay(transform.position, targetPositionCache - transform.position, Color.magenta);
        Vector3 direction = (targetPositionCache - transform.position).normalized;
        transform.Rotate(spinSpeed * Time.deltaTime * Vector3.forward, Space.Self);
        transform.Translate(travelSpeed * Time.deltaTime * direction, Space.World);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Hittable")) {
            Instantiate(vfx, transform.position, Quaternion.identity);
            SoundBoard.Instance.PlayOne(Audio.AudioEnum.MissileHitSFX, 1);
            foreach (Collider hitCollider in Physics.OverlapSphere(transform.position, 5)) {
                bool foundRb = hitCollider.TryGetComponent(out Rigidbody rb);
                if (foundRb) rb.AddExplosionForce(explosionPower, transform.position, explosionRadius);
            }
            Destroy(gameObject);
        }
    }
}
