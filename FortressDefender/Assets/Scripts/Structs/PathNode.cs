using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

/// <summary>
/// Structure corresponding to the global waypoint, used to calculate the path using pathfinding.
/// </summary>
[Serializable]
[BurstCompile]
public struct PathNode : IEquatable<PathNode>
{
    /// <summary>
    /// X position on grid.
    /// </summary>
    public int x;
    /// <summary>
    /// Y position on grid.
    /// </summary>
    public int y;
    /// <summary>
    /// Index on grid table.
    /// </summary>
    public int index;
    /// <summary>
    /// Value used in A* pathfinding algorithm. 
    /// gCost means the cost of moving to the current point from the starting point.
    /// </summary>
    public int gCost;
    /// <summary>
    /// Value used in A* pathfinding algorithm. 
    /// hCost means the cost of moving from the current point to the destination determined by the heuristics.
    /// </summary>
    public int hCost;
    /// <summary>
    /// Value used in A* pathfinding algorithm. 
    /// fCost is the total cost of moving from start point to destination point
    /// and it is equal to the sum of gCost and hCost.
    /// </summary>
    public int fCost;
    /// <summary>
    /// A variable that determines whether a pathnode can be crossed.
    /// </summary>
    public bool isWalkable;
    /// <summary>
    /// Index of previous node on the path.
    /// </summary>
    public int cameFromNodeIndex;
    /// <summary>
    /// Position of node in world coordinates.
    /// </summary>
    public float3 worldPosition;

    /// <summary>
    /// Pathnode constructor.
    /// </summary>
    /// <param name="x">X position on grid.</param>
    /// <param name="y">Y position on grid.</param>
    /// <param name="gridWidth">Width of grid.</param>
    /// <param name="isWalkable">Is a pathnode available</param>
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

    /// <summary>
    /// Pathnode constructor, based on waypoint.
    /// </summary>
    /// <param name="w">Waypoint.</param>
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

    /// <summary>
    /// Position of pathnode in grid coordinates.
    /// </summary>
    public int2 XY
    {
        get => new int2(x, y);
        set
        {
            x = value.x;
            y = value.y;
        }
    }

    /// <summary>
    /// Function that calculates the node index on a grid.
    /// </summary>
    /// <param name="gridWidth">Width of grid.</param>
    public void CalculateIndex(int gridWidth)
    {
        index = x + y * gridWidth;
    }

    /// <summary>
    /// Static function that calculates the node index on a grid.
    /// </summary>
    /// <param name="x">X position on grid.</param>
    /// <param name="y">Y position on grid.</param>
    /// <param name="gridWidth">Width of grid.</param>
    /// <returns></returns>
    public static int CalculateIndexOfNode(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    /// <summary>
    /// Static function that calculates the node index on a grid.
    /// </summary>
    /// <param name="gridPosition">Node position on grid.</param>
    /// <param name="gridWidth">Width of grid.</param>
    /// <returns></returns>
    public static int CalculateIndexOfNode(int2 gridPosition, int gridWidth)
    {
        return gridPosition.x + gridPosition.y * gridWidth;
    }

    /// <summary>
    /// Fuction that updates fCost of the node.
    /// </summary>
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }



    // ##############################
    // # Operators and comparators. #
    // ##############################

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