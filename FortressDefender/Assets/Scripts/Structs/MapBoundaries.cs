using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using System;


/// <summary>
/// Structure defining the limits of the grid. 
/// The min and max values mean the plane values, 
/// where the XY plane for int2 means the XZ plane in global coordinates.
/// </summary>
[BurstCompile]
[Serializable]
public struct MapBoundaries
{
    public int2 min;
    public int2 max;
}