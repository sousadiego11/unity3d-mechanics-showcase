using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] float verticalSensitivity = 1;
    [SerializeField] float horizontalSensitivity = 1;

    float pitchPos;
    float yawPos;

    void Update() {
        pitchPos += Input.GetAxis("Mouse Y"); // Change to be a universal input, regardles of the mouse
        pitchPos = Mathf.Clamp(pitchPos, -20f, 50f);

        yawPos += Input.GetAxis("Mouse X"); // Change to be a universal input, regardles of the mouse
        
        Quaternion rotationMatrix = Quaternion.Euler(pitchPos, yawPos, 0f);
        Vector3 newPositionOffset = Vector3.forward * positionOffset.z + Vector3.up * positionOffset.y + Vector3.right * positionOffset.x;
        Vector3 newPosition = target.transform.position - rotationMatrix * newPositionOffset;

        transform.SetPositionAndRotation(newPosition, rotationMatrix); 
    }
}
