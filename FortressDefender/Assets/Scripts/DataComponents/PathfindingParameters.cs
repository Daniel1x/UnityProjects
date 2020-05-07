using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System;

/// <summary>
/// Component that holds information about pathfinding start and end points.
/// </summary>
[GenerateAuthoringComponent]
public struct PathfindingParameters : IComponentData
{
    public bool needNewPath;
    public int2 startWorldPoint;
    public int2 endWorldPoint;
    public int2 startGridPoint;
    public int2 endGridPoint;
}
