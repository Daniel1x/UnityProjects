using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathNode : IEquatable<PathNode>
{
    private const int DIGITS = 0;

    public Vector3 position;

    public int gCost = int.MaxValue;
    public int hCost = int.MaxValue;
    public int fCost = int.MaxValue;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public PathNode previousNode = null;

    public PathNode(Vector3 worldPosition)
    {
        this.position = Round(worldPosition, DIGITS);
    }

    private Vector3 Round(Vector3 v, int digits)
    {
        float x = Round(v.x, digits);
        float y = Round(v.y, digits);
        float z = Round(v.z, digits);
        return new Vector3(x, y, z);
    }

    private float Round(float val, int digits)
    {
        float scaleFactor = Mathf.Pow(10, digits);
        float temp = Mathf.Round(val * scaleFactor) / scaleFactor;
        return temp;
    }

    public override bool Equals(object obj)
    {
        var node = obj as PathNode;
        return node != null &&
               position.Equals(node.position);
    }

    public bool Equals(PathNode other)
    {
        return other != null &&
               position.Equals(other.position);
    }

    public override int GetHashCode()
    {
        return 1206833562 + EqualityComparer<Vector3>.Default.GetHashCode(position);
    }

    public static bool operator ==(PathNode node1, PathNode node2)
    {
        return EqualityComparer<PathNode>.Default.Equals(node1, node2);
    }

    public static bool operator !=(PathNode node1, PathNode node2)
    {
        return !(node1 == node2);
    }
}