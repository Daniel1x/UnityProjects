using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator mapGenerator = (MapGenerator)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create fragment"))
        {
            mapGenerator.CreateMap();
        }
        if (GUILayout.Button("Reset Map"))
        {
            mapGenerator.ResetMap();
        }
        if (GUILayout.Button("Destroy Map"))
        {
            mapGenerator.DestroyMap();
        }

        GUILayout.EndHorizontal();

        if(GUILayout.Button("Create random fragment"))
        {
            mapGenerator.CreateRandomMapFragment();
        }
    }
}