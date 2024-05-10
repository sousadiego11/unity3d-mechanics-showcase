using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Vector3 frameOffset;

    float pitchPos;
    float yawPos;


    void Update() {
        pitchPos += Input.GetAxis("Mouse Y"); // Change to be a universal input, regardles of the mouse
        yawPos += Input.GetAxis("Mouse X"); // Change to be a universal input, regardles of the mouse
        
        Quaternion rotationMatrix = Quaternion.Euler(Mathf.Clamp(pitchPos, -20f, 50f), yawPos, 0f);
        Vector3 newPosition = target.transform.position - rotationMatrix * (Vector3.forward * frameOffset.z + Vector3.up * -frameOffset.y + Vector3.right * frameOffset.x);

        transform.SetPositionAndRotation(newPosition, rotationMatrix); 
    }
}
