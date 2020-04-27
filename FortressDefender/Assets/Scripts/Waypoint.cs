using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;


[Serializable]
public struct Waypoint
{
    public int2 gridPosition;
    public int2 worldPosition;
    public int index;
    public bool isWalkable;
}


/*
[Serializable]
public class Waypoint
{
    public bool isExplored = false;
    public Waypoint exploredFrom = null;
    public Vector2 position2D = new Vector2();
    public Vector3 position = new Vector3();

    public Waypoint(Vector3 position)
    {
        this.position = position;
        this.position2D = new Vector2(position.x, position.z);
    }
}
*/
