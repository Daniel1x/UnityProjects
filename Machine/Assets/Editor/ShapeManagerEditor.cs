using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeManager))]
public class ShapeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ShapeManager shapeManager = (ShapeManager)target;

        GUILayout.Label("Mesh to/from scriptable object");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            shapeManager.SaveNewShapeAsScriptableObject();
        }
        if (GUILayout.Button("Load"))
        {
            shapeManager.LoadShapeFromScriptableObject();
        }
        if (GUILayout.Button("Restart"))
        {
            shapeManager.RestartMeshGenerator();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Copy Data"))
        {
            shapeManager.CopyData(shapeManager.dataFrom, shapeManager.dataTo);
        }
        if (GUILayout.Button("Set Target Shape"))
        {
            shapeManager.SetTargetShape();
        }

    }
}
