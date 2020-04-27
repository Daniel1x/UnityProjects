using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

[GenerateAuthoringComponent]
public struct PathfindingParameters : IComponentData
{
    public int2 startGridPoint { get => WaypointsManager.GetGridPositionFromWorldPosition(startWorldPoint); }
    public int2 endGridPoint { get => WaypointsManager.GetGridPositionFromWorldPosition(endWorldPoint); }

    public int2 startWorldPoint;
    public int2 endWorldPoint;
}
