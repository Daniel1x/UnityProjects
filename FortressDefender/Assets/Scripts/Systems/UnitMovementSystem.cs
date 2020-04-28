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
        Entities.ForEach((Entity entity, ref Translation position, ref UnitData unitData, ref TargetPositionData targetData) =>
        {
            EntityManager.AddComponentData(entity, new PathfindingParameters { needNewPath = true,
                                                                               startWorldPoint = WaypointsManager.Func.RoundToInt2(position.Value),
                                                                               endWorldPoint = targetData.endWorldPosition });
            PostUpdateCommands.RemoveComponent<TargetPositionData>(entity);
        });
        
        Entities.ForEach((Entity entity, DynamicBuffer<PathPositionsBuffer> path, ref Translation position, ref UnitData unitData) =>
        {
            if (unitData.pathIndex >= 0)
            {
                if (math.distance(position.Value, path[unitData.pathIndex].worldPosition) < 0.1f) unitData.pathIndex--;

                if (unitData.pathIndex >= 0)
                {
                    float3 pathPosition = path[unitData.pathIndex].worldPosition;
                    float3 moveDirection = math.normalize(pathPosition - position.Value);
                    position.Value += moveDirection * unitData.unitSpeed * Time.DeltaTime;
                }
            }
            else
            {
                EntityManager.AddComponentData(entity, new TargetPositionData { endWorldPosition = WaypointsManager.Func.GetRandomWalkableWaypoint() });
            }
        });
    }
}
