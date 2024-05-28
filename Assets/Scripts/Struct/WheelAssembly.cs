using System;
using UnityEngine;

[Serializable]
public struct WheelAssembly {
    public Spring spring;
    public Tire tire;
}

[Serializable]
public struct Spring {
    [SerializeField] public Transform transform;
    [SerializeField] public float strength;
    [SerializeField] public float damping;
    [SerializeField] public float restDist;
}

[Serializable]
public struct Tire {
    [SerializeField] public Transform transform;
    [SerializeField] [Range(0, 1)] public float friction;
    [SerializeField] [Range(0, 15)] public float maxAngle;
}