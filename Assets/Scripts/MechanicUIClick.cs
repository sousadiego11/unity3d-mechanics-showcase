using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MechanicUIClick : MonoBehaviour
{
    [SerializeField] Mechanic mechanic;
    [SerializeField] List<Mechanic> toDisable;
    bool enabledByUser = false;
    

    public void OnClick() {
        enabledByUser = !enabledByUser;
        if (enabledByUser) {
            foreach (Mechanic mech in toDisable) mech.Disable();
            mechanic.Enable();
        } else {
            mechanic.Disable();
        }
    }
}
