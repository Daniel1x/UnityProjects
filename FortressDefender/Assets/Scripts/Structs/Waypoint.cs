using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

/// <summary>
/// Structure that defines the waypoint properties.
/// </summary>
[Serializable]
public struct Waypoint
{
    /// <summary>
    /// Position of waypoint on global grid.
    /// </summary>
    public int2 gridPosition;
    /// <summary>
    /// Position of the point at world coordinates. 
    /// The X and Y values of this variable define the values of vector3 (or float3), 
    /// where v3.x = this.x and v3.z = this.y (because v3.y = 0 flattens the plane)
    /// </summary>
    public int2 worldPosition;
    /// <summary>
    /// Index of current waypoint in grid table.
    /// </summary>
    public int index;
    /// <summary>
    /// A variable that determines whether a waypoint can be crossed.
    /// </summary>
    public bool isWalkable;
    /// <summary>
    /// Position of waypoint in world coordinates, as float3.
    /// </summary>
    public float3 worldPosition3f { get => new float3(worldPosition.x, 0f, worldPosition.y); }
}