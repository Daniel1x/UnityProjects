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
        if (Input.GetMouseButtonDown(0))
        {
            Entities.ForEach((Entity entity, ref Translation position, ref UnitData unitData, ref TargetPositionData targetData) =>
            {
                EntityManager.AddComponentData(entity, new PathfindingParameters
                {
                    startWorldPoint = targetData.startWorldPosition,
                    endWorldPoint = targetData.endWorldPosition
                });
            });
        }

        Entities.ForEach((DynamicBuffer<PathPositionsBuffer> path, ref Translation position, ref UnitData unitData) =>
        {
            if (unitData.pathIndex >= 0)
            {
                float3 pathPosition = path[unitData.pathIndex].worldPosition;
                float3 moveDirection = math.normalize(pathPosition - position.Value);
                position.Value += moveDirection * unitData.unitSpeed * Time.DeltaTime;

                if (math.distance(position.Value, pathPosition) < 0.1f) unitData.pathIndex--;
            }
        });
    }
}
