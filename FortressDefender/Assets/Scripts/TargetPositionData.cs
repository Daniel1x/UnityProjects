using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetPositionData : IComponentData
{
    public int2 startWorldPosition;
    public int2 endWorldPosition;
}
