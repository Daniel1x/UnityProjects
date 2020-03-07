using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMeshInfo : MonoBehaviour
{
    public int numberOfVerticesPerLayer = 12;
    public int numberOfLayers = 10;
    public float hightOfOneLayer = 0.1f;
    public float widthOfCylinder = 1f;
    public float midpointHeightDifference = 0.1f;

    public void SetInformations(int numberOfVerticesPerLayer, int numberOfLayers, float hightOfOneLayer, float widthOfCylinder, float midpointHeightDifference)
    {
        this.numberOfVerticesPerLayer = numberOfVerticesPerLayer;
        this.numberOfLayers = numberOfLayers;
        this.hightOfOneLayer = hightOfOneLayer;
        this.widthOfCylinder = widthOfCylinder;
        this.midpointHeightDifference = midpointHeightDifference;
    }
}
