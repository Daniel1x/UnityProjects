using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[Serializable]
[BurstCompile]
public struct PathNode : IEquatable<PathNode>
{
    public int x;
    public int y;

    public int index;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;

    public int cameFromNodeIndex;

    public float3 worldPosition;

    public PathNode(int x, int y, int gridWidth, bool isWalkable) : this()
    {
        this.x = x;
        this.y = y;
        this.CalculateIndex(gridWidth);
        this.gCost = int.MaxValue;
        this.hCost = int.MaxValue;
        this.CalculateFCost();
        this.isWalkable = isWalkable;
        this.cameFromNodeIndex = -1;
    }

    public PathNode(Waypoint w)
    {
        this.x = w.gridPosition.x;
        this.y = w.gridPosition.y;
        this.index = w.index;
        this.gCost = int.MaxValue;
        this.hCost = int.MaxValue;
        this.fCost = int.MaxValue;
        this.isWalkable = w.isWalkable;
        this.cameFromNodeIndex = -1;
        this.worldPosition = new float3(w.worldPosition.x, 0f, w.worldPosition.y);
    }

    public int2 XY
    {
        get => new int2(x, y);
        set
        {
            x = value.x;
            y = value.y;
        }
    }

    public void CalculateIndex(int gridWidth)
    {
        index = x + y * gridWidth;
    }

    public static int CalculateIndexOfNode(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    public static int CalculateIndexOfNode(int2 gridPosition, int gridWidth)
    {
        return gridPosition.x + gridPosition.y * gridWidth;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public bool Equals(PathNode other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public static bool operator ==(PathNode node1, PathNode node2)
    {
        return node1.x == node2.x && node1.y == node2.y;
    }

    public static bool operator !=(PathNode node1, PathNode node2)
    {
        return node1.x != node2.x || node1.y != node2.y;
    }
}