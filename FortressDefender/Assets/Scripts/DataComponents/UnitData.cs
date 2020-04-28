using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct UnitData : IComponentData
{
    public int pathIndex;
    public float unitSpeed;
}
