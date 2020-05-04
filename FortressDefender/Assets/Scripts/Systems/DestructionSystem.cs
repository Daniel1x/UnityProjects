using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public class DestructionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        int destroyedEntities = 0;
        Entities.ForEach((Entity entity, ref LifetimeData lifetimeData) => 
        {
            if (!lifetimeData.alive)
            {
                destroyedEntities++;
                EntityManager.DestroyEntity(entity);
            }
        });
        ECSManager.spawnedEntities -= destroyedEntities;
    }
}
