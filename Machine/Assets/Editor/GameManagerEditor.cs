using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager gameManager = (GameManager)target;

        if(GUILayout.Button("Reset level"))
        {
            Debug.Log("Nothing to load!");
            //gameManager.ResetLevel();
        }
        if(GUILayout.Button("Save current level"))
        {
            gameManager.SaveLevelToScriptableObject();
        }
        if(GUILayout.Button("Create cylinder"))
        {
            StaticCylinderCreator.CreateCylinder(FindObjectOfType<GameManager>().transform, "Cylinder", "GeneratedCylinder", Vector3.zero, 12, 101, 3f, 0.025f, 0.1f, "Metal");
        }
    }
}
