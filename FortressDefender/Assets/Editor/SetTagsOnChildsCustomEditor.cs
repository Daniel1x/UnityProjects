using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetTagsOnChilds))]
public class SetTagsOnChildsCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SetTagsOnChilds setTagsOnChilds = (SetTagsOnChilds)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Set Tags"))
        {
            setTagsOnChilds.SetTags();
        }
        if (GUILayout.Button("Set Custom Tags"))
        {
            setTagsOnChilds.SetCustomTags();
        }

        GUILayout.EndHorizontal();
    }
}
