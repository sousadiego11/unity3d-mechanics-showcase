using System;
using UnityEngine;

[Serializable]
public struct WheelAssembly {
    public SpringTip springTip;
    public Tire tire;
}

[Serializable]
public struct SpringTip {
    [SerializeField] public Transform transform;
    [SerializeField] public float strength;
    [SerializeField] public float damping;
    [SerializeField] public float restDist;
    [SerializeField] public float maxDist;
}

[Serializable]
public struct Tire {
    [SerializeField] public Transform transform;
    [SerializeField] public MeshRenderer mesh;
    [SerializeField] public Position position;
    [SerializeField] public float maxSteerAngle;
    [SerializeField] [Range(0, 1)] public float grip;
    public enum Position {
        FL,
        FR,
        BL,
        BR
    }
}