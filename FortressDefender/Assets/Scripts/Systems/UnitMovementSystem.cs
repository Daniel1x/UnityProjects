using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;

public class UnitMovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Random value used to finding target point.
        bool randomCondition = UnityEngine.Random.Range(0f, 1f) > 0.5f;

        Entities.ForEach((Entity entity, DynamicBuffer<PathPositionsBuffer> path, ref Translation position, ref UnitData unitData) =>
        {
            // if there is pathIndex
            if (unitData.pathIndex >= 0) // Follow path.
            {
                // Check if close enough, and select next pathIndex.
                if (math.distance(position.Value, path[unitData.pathIndex].worldPosition) < 0.1f) unitData.pathIndex--;

                // If there is still waypoint visit.
                if (unitData.pathIndex >= 0)
                {
                    // Move to target point.
                    float3 pathPosition = path[unitData.pathIndex].worldPosition;
                    float3 moveDirection = math.normalize(pathPosition - position.Value);
                    position.Value += moveDirection * unitData.unitSpeed * Time.DeltaTime;
                }
            }
            else // if unitData.pathIndex = -1, so there is no path.
            {
                // Find new path.
                EntityManager.AddComponentData(entity, new PathfindingParameters { needNewPath = true,
                                                                                   startWorldPoint = WaypointsManager.Func.RoundToGrid(position.Value),
                                                                                   endWorldPoint = randomCondition ?
                                                                                   WaypointsManager.Func.GetTargetPosition() :
                                                                                   WaypointsManager.Func.GetRandomWalkableWaypoint()
                                                                                 });
            }
        });
    }
}
