using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

[UpdateAfter(typeof(UnitMovementSystem))]
public class DestrucionSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        commandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer.Concurrent commands = commandBufferSystem.CreateCommandBuffer().ToConcurrent();

        JobHandle jobHandle = Entities.WithChangeFilter<LifetimeData>().ForEach((Entity entity, int entityInQueryIndex, ref LifetimeData lifeTimeData) =>
        {
            if (!lifeTimeData.alive)
            {
                commands.DestroyEntity(entityInQueryIndex, entity);
            }
        }).Schedule(inputDeps);
        
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        
        return jobHandle;
    }
}
