using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct LifetimeData : IComponentData
{
    /// <summary>
    /// Is an entity still alive?
    /// </summary>
    public bool alive;
}
