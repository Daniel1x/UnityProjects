using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArrangeChildComponentsEvenly))]
public class ArrangeChildComponentsEvenlyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ArrangeChildComponentsEvenly arrChCompEv = (ArrangeChildComponentsEvenly)target;

        if (GUILayout.Button("Set Childs Positions"))
        {
            arrChCompEv.ChangeChildsPosition();
        }
    }
}
