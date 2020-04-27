using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Directions
{
    public static Vector2Int up = Vector2Int.up;
    public static Vector2Int down = Vector2Int.down;
    public static Vector2Int right = Vector2Int.right;
    public static Vector2Int left = Vector2Int.left;
    public static Vector2Int upRight = Vector2Int.one;
    public static Vector2Int upLeft = new Vector2Int(-1, 1);
    public static Vector2Int downRight = new Vector2Int(1, -1);
    public static Vector2Int downLeft = new Vector2Int(-1, -1);

    public const int arraySize = 8;

    public static Vector2Int[] directions = new Vector2Int[8] 
                                { up, down, right, left, upRight, downRight, downLeft, upLeft };

    public static Vector2Int[] directionsArray()
    {
        Vector2Int[] array = new Vector2Int[arraySize];
        array[0] = up;
        array[1] = right;
        array[2] = down;
        array[3] = left;
        array[4] = upRight;
        array[5] = downRight;
        array[6] = downLeft;
        array[7] = upLeft;
        return array;
    }
}
