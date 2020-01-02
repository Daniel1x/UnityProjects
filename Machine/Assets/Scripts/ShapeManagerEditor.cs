using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeManager))]
public class ShapeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        base.OnInspectorGUI();

        ShapeManager shapeManager = (ShapeManager)target;
        if (GUILayout.Button("Save New Shape"))
        {
            shapeManager.SaveNewShapeToFile();
        }
    }
}
