using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(128)]
public struct PathPositionsBuffer : IBufferElementData
{
    public float3 worldPosition;
}
