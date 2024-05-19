using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : Mechanic
{
    [SerializeField] PlayerController player;
    [SerializeField] List<CameraFollowProps> properties;

    float pitchPos;
    float yawPos;

    void Update() {
        if (!Locked()) {
            pitchPos += Input.GetAxis("Mouse Y") * -1; // Change to be a universal input, regardles of the mouse
            pitchPos = Mathf.Clamp(pitchPos, -20f, 50f);

            yawPos += Input.GetAxis("Mouse X"); // Change to be a universal input, regardles of the mouse
        }
        
        Quaternion rotationMatrix = Quaternion.Euler(pitchPos, yawPos, 0f);
        Vector3 newPositionOffset = Vector3.forward * GetPropertiesStrategy().offset.z + Vector3.up * GetPropertiesStrategy().offset.y + Vector3.right * GetPropertiesStrategy().offset.x;
        Vector3 newPosition = player.transform.position - rotationMatrix * newPositionOffset;

        transform.SetPositionAndRotation(newPosition, rotationMatrix);
    }

    CameraFollowProps GetPropertiesStrategy() {
        if (player.isRunning) {
            return properties.Find(m => m.identifier == CameraFollowProps.NameEnum.Running);
        } else if (player.isAiming) {
            return properties.Find(m => m.identifier == CameraFollowProps.NameEnum.Aiming);
        } else {
            return properties.Find(m => m.identifier == CameraFollowProps.NameEnum.Default);
        }
    }
}
