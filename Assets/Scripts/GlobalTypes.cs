using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum WallPosition
{
    LeftWall,
    TopWall,
    RightWall,
    BottomWall
}

[System.Serializable]
public class PointEvent : UnityEvent<WallPosition> { }