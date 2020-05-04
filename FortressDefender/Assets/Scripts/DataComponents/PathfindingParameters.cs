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
[BurstCompile]
public struct PathfindingParameters : IComponentData
{
    public int2 startGridPoint { get => WaypointsManager.Func.GetGridPositionFromWorldPosition(startWorldPoint); }
    public int2 endGridPoint { get => WaypointsManager.Func.GetGridPositionFromWorldPosition(endWorldPoint); }

    public bool needNewPath;
    public int2 startWorldPoint;
    public int2 endWorldPoint;
}
