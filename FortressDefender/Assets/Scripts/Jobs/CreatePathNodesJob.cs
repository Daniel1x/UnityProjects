using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;


/// <summary>
/// Paraller job for creating path nodes from waypoints.
/// </summary>
public struct CreatePathNodesJob : IJobParallelFor
{
    public NativeArray<PathNode> pathNodes;
    [ReadOnly] public NativeArray<Waypoint> waypoints;

    public void Execute(int index)
    {
        pathNodes[index] = new PathNode(waypoints[index]);
    }
}