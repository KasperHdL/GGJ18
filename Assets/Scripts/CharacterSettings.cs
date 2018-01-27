using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "", order = 1)]
public class CharacterSettings : ScriptableObject {
    public float moveForce;

    public float innerOuterDeadzone;
    public float wheelIsInnerForce;
    public float wheelIsOuterForce;
    public float wheelStraightForce;

    public float reverseMaxDot;
    public float reverseOffset;

    public AnimationCurve frontWheelForce;
    public AnimationCurve backWheelForce;

    public float onGroundCheckDistance;

    public float flipMinVelocity;
    public float flipDelay;

}
