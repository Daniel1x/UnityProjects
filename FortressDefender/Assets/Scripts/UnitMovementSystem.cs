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
            Entities.ForEach((Entity entity, ref Translation position, ref UnitData unitData) =>
            {
                EntityManager.AddComponentData(entity, new PathfindingParameters
                {
                    startPoint = new int2(0, 0),
                    endPoint = new int2(2, 2)
                });
            });
        }
    }
}
