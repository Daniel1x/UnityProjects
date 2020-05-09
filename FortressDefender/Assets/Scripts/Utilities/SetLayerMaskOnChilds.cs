using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLayerMaskOnChilds : MonoBehaviour
{
    [SerializeField] private string chosenLayerMaskName = "";
    [SerializeField] private string nameOfGameObject = "";

    public void SetLayerMasks()
    {
        SetLayerMasksOnChilds(transform);
    }

    /// <summary>
    /// Function that sets chosen layer mask to game objects with specific name.
    /// </summary>
    /// <param name="parent">Parent transform.</param>
    private void SetLayerMasksOnChilds(Transform parent)
    {
        foreach(Transform child in parent)
        {
            if (child.name == nameOfGameObject) child.gameObject.layer = LayerMask.NameToLayer(chosenLayerMaskName);
            SetLayerMasksOnChilds(child);
        }
    }
}
