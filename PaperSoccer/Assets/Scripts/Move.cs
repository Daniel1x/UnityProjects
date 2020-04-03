using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Move
{
    public Vector2Int from;
    public Vector2Int to;

    public Move(Vector2Int from, Vector2Int to)
    {
        this.from = from;
        this.to = to;
    }

    public Move Rotated()
    {
        return new Move(to, from);
    }

    public bool Compare(Move move)
    {
        return (this.from == move.from && this.to == move.to);
    }
}
