﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pathfinding))]
public class PathfindingCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Pathfinding pathfinding = (Pathfinding)target;
        
        if (GUILayout.Button("Create Path Nodes Array"))
        {
            pathfinding.CreateNodes();
        }
        if(GUILayout.Button("Find Path"))
        {
            pathfinding.FindPathToThePoint();
        }
    }


    /*
    public override void OnInspectorGUI()
    {
        Pathfinding pathfinding = (Pathfinding)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Calculate path"))
        {
            pathfinding.FindPath();
        }
        if(GUILayout.Button("Show walkable"))
        {
            pathfinding.ShowWalkableNodes();
        }

        GUILayout.EndHorizontal();
        
        base.OnInspectorGUI();
    }
    */
}