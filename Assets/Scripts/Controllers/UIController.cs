using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {
    [SerializeField] Canvas canvas;
    [SerializeField] Canvas playerCanvas;

    public static UIController Instance {get; private set;}


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    void Update() {
        canvas.enabled = Input.GetKey(KeyCode.Tab);
    }
    public bool VisibleUI() {
        return canvas.enabled;
    }

    public void PlayerUI(bool visible) {
        playerCanvas.enabled = visible;
    }
}
