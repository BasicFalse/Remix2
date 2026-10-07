using System;
using UnityEngine;

public enum PlayerType
{
    UnInit, Invalid, Teleport, Joystick, Physical
}

[Serializable]
public class ResourcesContainer
{
    [SerializeField]
    public MaterialDatas materialData;
    public Rigidbody RB;
}

[Serializable]
public class TransformContainer
{
    [SerializeField]
    public Transform PlayerRoot;
    public Transform Camera;
    public Transform Body;
    public Transform RightController;
    public Transform LeftController;
}

[Serializable]
public class BoolContainer
{
    [SerializeField]
    public bool isFlashlightOn;
    public bool isGettingJumped;
}

[Serializable]
public class ColliderContainer
{
    [SerializeField]
    public Collider headCollider;
    public CapsuleCollider bodyCollider;
}