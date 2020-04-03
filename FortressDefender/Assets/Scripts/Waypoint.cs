using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
