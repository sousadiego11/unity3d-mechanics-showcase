using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : Mechanic
{
    [SerializeField] PlayerController player;
    [SerializeField] float zoomSpeed;
    [SerializeField] List<CameraFollowProps> properties;

    float pitchPos;
    float yawPos;
    Vector3 offset;

    void Update() {
        if (!Locked()) {
            pitchPos += Input.GetAxis("Mouse Y") * -1; // Change to be a universal input, regardles of the mouse
            pitchPos = Mathf.Clamp(pitchPos, -20f, 50f);

            yawPos += Input.GetAxis("Mouse X"); // Change to be a universal input, regardles of the mouse
        }
        
        LerpOffset();
        Quaternion rotationMatrix = Quaternion.Euler(pitchPos, yawPos, 0f);
        Vector3 newPositionOffset = Vector3.forward * offset.z + Vector3.up * offset.y + Vector3.right * offset.x;
        Vector3 newPosition = player.transform.position - rotationMatrix * newPositionOffset;

        transform.SetPositionAndRotation(newPosition, rotationMatrix);
    }

    void LerpOffset() {
        CameraFollowProps currentProps;

        if (player.isRunning && !player.isAiming) {
            currentProps = properties.Find(m => m.identifier == CameraFollowProps.NameEnum.Running);
        } else if (player.isAiming) {
            currentProps = properties.Find(m => m.identifier == CameraFollowProps.NameEnum.Aiming);
        } else {
            currentProps = properties.Find(m => m.identifier == CameraFollowProps.NameEnum.Default);
        }

        offset = Vector3.Lerp(offset, currentProps.offset, Time.deltaTime * zoomSpeed);
    }
}
