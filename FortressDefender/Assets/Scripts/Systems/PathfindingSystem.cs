using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Transforms;
using System;
using Unity.Jobs.LowLevel.Unsafe;

public class PathfindingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (WaypointsManager.Instance == null) return; // If there is no WaypointManager.Instance return.

        // List of jobs.
        NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);

        // If the waypoints have been modified, the path nodes and the global path must be updated.
        if (WaypointsManager.waypointHasBeenModified) WaypointsManager.Func.UpdatePathNodesNativeArray();

        ComponentDataFromEntity<UnitData> unitDataComponentFromEntity = GetComponentDataFromEntity<UnitData>();

        Entities.ForEach((Entity entity, DynamicBuffer<PathPositionsBuffer> pathPositionsBuffer, ref PathfindingParameters parameters) =>
        {
            if (parameters.needNewPath) // If new path needs to be created
            {
                // Create pathfinding job for current entity.
                FindPathJob findPathJob = new FindPathJob
                {
                    startGridPosition = parameters.startGridPoint,
                    endGridPosition = parameters.endGridPoint,
                    gridSize = WaypointsManager.GridSize,
                    pathNodes = new NativeArray<PathNode>(WaypointsManager.pathNodes, Allocator.TempJob),
                    pathPositionsBuffer = pathPositionsBuffer,
                    entity = entity,
                    unitDataComponentFromEntity = unitDataComponentFromEntity
                };
                // Schedule current pathfinding job.
                jobHandles.Add(findPathJob.Schedule());
                // Mark that there is no need to create new path.
                parameters.needNewPath = false;
            }
        });

        // Complete all pathfinding jobs.
        JobHandle.CompleteAll(jobHandles);
    }
}
