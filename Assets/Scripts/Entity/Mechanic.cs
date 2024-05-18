using System.Collections.Generic;
using UnityEngine;

public class Mechanic : MonoBehaviour {

    public bool Locked() {
        return UIController.Instance != null && UIController.Instance.VisibleUI();
    }
    public void Enable() {
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }
}