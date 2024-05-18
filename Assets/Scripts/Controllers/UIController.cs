using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {
    [SerializeField] Canvas canvas;

    public static UIController Instance {get; private set;}

    public bool VisibleUI() {
        return canvas.enabled;
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    void Update() {
        if (Input.GetKey(KeyCode.Tab)) {
            canvas.enabled = true;
        } else {
            canvas.enabled = false;
        }
    }
}
