using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Directions 
{
    public const int nDirections = 8;

    private static float gridStep = 1f;

    public static Vector2[] directions = new Vector2[nDirections];

    public static void SetGridStep(float gridStep)
    {
        Directions.gridStep = gridStep;
        CreateDirectionsArray(gridStep);
    }

    private static void CreateDirectionsArray(float gridStep)
    {
        directions[0] = Vector2.up;
        directions[1] = Vector2.one;
        directions[2] = Vector2.right;
        directions[3] = new Vector2(1f, -1f);
        directions[4] = Vector2.down;
        directions[5] = -Vector2.one;
        directions[6] = Vector2.left;
        directions[7] = new Vector2(-1f, 1f);

        for (int i = 0; i < nDirections; i++)
        {
            directions[i] *= gridStep;
        }
    }
}
