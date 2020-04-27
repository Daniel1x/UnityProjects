using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

[GenerateAuthoringComponent]
public struct PathfindingParameters : IComponentData
{
    public int2 startPoint;
    public int2 endPoint;
}
