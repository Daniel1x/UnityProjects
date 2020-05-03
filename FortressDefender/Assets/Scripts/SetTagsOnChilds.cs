using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that helps to re-tag childs of created map.
/// </summary>
public class SetTagsOnChilds : MonoBehaviour
{
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private string floorTag = "Floor";
    [SerializeField] private string nameOfWall = "Cube(Clone)";
    [SerializeField] private string nameOfFloor = "Floor";

    public void SetTags()
    {
        SetTagOnChilds(this.transform);
    }

    /// <summary>
    /// Function that sets appropriate tags on child game objects.
    /// </summary>
    /// <param name="parent">Parent gameobject.</param>
    private void SetTagOnChilds(Transform parent)
    {
        foreach(Transform child in parent)
        {
            if (child.name == nameOfWall) child.tag = wallTag;
            else if (child.name == nameOfFloor) child.tag = floorTag;
            SetTagOnChilds(child);
        }
    }
}