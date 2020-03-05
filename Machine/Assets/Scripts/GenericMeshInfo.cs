using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMeshInfo : MonoBehaviour
{
    private int numberOfVerticesPerLayer = 12;
    private int numberOfLayers = 10;
    private float hightOfOneLayer = 0.1f;
    private float widthOfCylinder = 1f;
    private float midpointHeightDifference = 0.1f;

    public int NumberOfVerticesPerLayer { get => numberOfVerticesPerLayer; }
    public int NumberOfLayers { get => numberOfLayers; }
    public float HightOfOneLayer { get => hightOfOneLayer; }
    public float WidthOfCylinder { get => widthOfCylinder; }
    public float MidpointHeightDifference { get => midpointHeightDifference; }

    public void SetInformations(int numberOfVerticesPerLayer, int numberOfLayers, float hightOfOneLayer, float widthOfCylinder, float midpointHeightDifference)
    {
        this.numberOfVerticesPerLayer = numberOfVerticesPerLayer;
        this.numberOfLayers = numberOfLayers;
        this.hightOfOneLayer = hightOfOneLayer;
        this.widthOfCylinder = widthOfCylinder;
        this.midpointHeightDifference = midpointHeightDifference;
    }
}
