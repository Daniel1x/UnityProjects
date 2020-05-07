using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;

public class UnitMovementSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        commandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // Create command buffer.
        EntityCommandBuffer.Concurrent commandBuffer = commandBufferSystem.CreateCommandBuffer().ToConcurrent();

        float deltaTime = Time.DeltaTime;

        // Checking if the target waypoint is available for pathfinding.
        Waypoint targetWaypoint = WaypointsManager.Func.GetTargetWaypoint();
        bool isTargetWaypointWalkable = targetWaypoint.isWalkable;
        int2 targetWorldPosition = targetWaypoint.worldPosition;

        // Get size of map.
        MapBoundries boundries = WaypointsManager.MapBoundries;

        JobHandle jobHandle = Entities.WithName("UnitMovementSystem")
            .ForEach((int entityInQueryIndex, Entity entity, DynamicBuffer<PathPositionsBuffer> path, ref Translation position, ref UnitData unitData, ref LifetimeData lifetimeData) =>
            {
                // if there is pathIndex
                if (unitData.pathIndex >= 0) // Follow path.
                {
                    // Check if close enough, and select next pathIndex.
                    if (math.distance(position.Value, path[unitData.pathIndex].worldPosition) < unitData.unitSpeed * deltaTime) unitData.pathIndex--;

                    // If there is still waypoint to visit.
                    if (unitData.pathIndex >= 0)
                    {
                        // Move to target point.
                        float3 pathPosition = path[unitData.pathIndex].worldPosition;
                        float3 moveDirection = math.normalize(pathPosition - position.Value);
                        position.Value += moveDirection * unitData.unitSpeed * deltaTime;
                    }
                    else
                    {
                        // Check distance to target waypoint.
                        if (math.distance(position.Value, targetWaypoint.worldPosition3f) < unitData.unitSpeed * deltaTime)
                        {
                            // Reached end of the path.
                            lifetimeData.alive = false;
                        }
                        else
                        {
                            // Target waypoint has been moved.
                        }
                    }
                }
                else // if unitData.pathIndex = -1, so there is no path.
                {
                    // If there is a valid target waypoint, add pathfinding parameters.
                    if (isTargetWaypointWalkable)
                    {
                        int2 startWorldPos = WaypointsManager.Func.RoundToGrid(position.Value);
                        PathfindingParameters pathfindingParameters = new PathfindingParameters
                        {
                            needNewPath = true,
                            startWorldPoint = startWorldPos,
                            endWorldPoint = targetWorldPosition,
                            startGridPoint = WaypointsManager.Func.GetGridPositionFromWorldPosition(startWorldPos, boundries),
                            endGridPoint = WaypointsManager.Func.GetGridPositionFromWorldPosition(targetWorldPosition, boundries)
                        };
                        commandBuffer.AddComponent(entityInQueryIndex, entity, pathfindingParameters);
                    }
                }
            }).Schedule(inputDeps);

        commandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}