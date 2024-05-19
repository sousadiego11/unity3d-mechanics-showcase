using System;
using UnityEngine;

[Serializable]
public struct CameraFollowProps {
    public Vector3 offset; // x 0, y -1.5, z 3.2
    public NameEnum identifier;
    
    public enum NameEnum {
        Default,
        Running,
        Aiming
    }
}