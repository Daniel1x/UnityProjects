using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetLayerMaskOnChilds))]
public class SetLayerMaskOnChildsCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SetLayerMaskOnChilds setLayerMaskOnChilds = (SetLayerMaskOnChilds)target;

        if(GUILayout.Button("Set LayerMask"))
        {
            setLayerMaskOnChilds.SetLayerMasks();
        }
    }
}
