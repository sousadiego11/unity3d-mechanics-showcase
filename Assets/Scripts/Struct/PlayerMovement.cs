using System;

[Serializable]
public struct PlayerMovement {
    public float maximumSpeed; //5
    public float acceleration; //5
    public float deceleration; //10
    public float rotationSpeed; //400
    public float fallSpeed; //10
    public NameEnum identifier;
    
    public enum NameEnum {
        Default,
        Running,
        Aiming
    }
}