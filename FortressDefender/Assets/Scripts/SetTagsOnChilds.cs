using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void SetTagOnChilds(Transform parent)
    {
        foreach(Transform child in parent)
        {
            if (child.name == nameOfWall) child.tag = wallTag;
            if (child.name == nameOfFloor) child.tag = floorTag;
            SetTagOnChilds(child);
        }
    }
}