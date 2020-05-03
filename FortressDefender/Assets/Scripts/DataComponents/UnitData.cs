using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct UnitData : IComponentData
{
    /// <summary>
    /// Index of current path node.
    /// </summary>
    public int pathIndex;
    /// <summary>
    /// Movement speed of unit.
    /// </summary>
    public float unitSpeed;
}
