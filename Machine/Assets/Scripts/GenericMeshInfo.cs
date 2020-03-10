using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Info
{
    public int numberOfVerticesPerLayer = 0;
    public int numberOfLayers = 0;
    public float hightOfOneLayer = 0f;
    public float widthOfCylinder = 0f;
    public float midpointHeightDifference = 0f;
    public float[] magnitudesOfLayers = new float[0];

    public Info() {}

    public Info(Info info)
    {
        numberOfVerticesPerLayer = info.numberOfVerticesPerLayer;
        numberOfLayers = info.numberOfLayers;
        hightOfOneLayer = info.hightOfOneLayer;
        widthOfCylinder = info.widthOfCylinder;
        midpointHeightDifference = info.midpointHeightDifference;
        magnitudesOfLayers = CopyArray(info.magnitudesOfLayers);
    }

    public Info(int numberOfVerticesPerLayer, int numberOfLayers, float hightOfOneLayer, float widthOfCylinder, float midpointHeightDifference, float[] magnitudesOfLayers)
    {
        this.numberOfVerticesPerLayer = numberOfVerticesPerLayer;
        this.numberOfLayers = numberOfLayers;
        this.hightOfOneLayer = hightOfOneLayer;
        this.widthOfCylinder = widthOfCylinder;
        this.midpointHeightDifference = midpointHeightDifference;
        this.magnitudesOfLayers = CopyArray(magnitudesOfLayers);
    }

    private float[] CopyArray(float[] arrayToCopy)
    {
        int arrayLength = arrayToCopy.Length;
        float[] newArray = new float[arrayLength];
        for (int i = 0; i < arrayLength; i++)
            newArray[i] = arrayToCopy[i];
        return newArray;
    }
}

public class GenericMeshInfo : MonoBehaviour
{
    public Info info = new Info();
    
    public void SetInformations(int numberOfVerticesPerLayer, int numberOfLayers, float hightOfOneLayer, float widthOfCylinder, float midpointHeightDifference, float[] magnitudesOfLayers)
    {
        info.numberOfVerticesPerLayer = numberOfVerticesPerLayer;
        info.numberOfLayers = numberOfLayers;
        info.hightOfOneLayer = hightOfOneLayer;
        info.widthOfCylinder = widthOfCylinder;
        info.midpointHeightDifference = midpointHeightDifference;
        info.magnitudesOfLayers = magnitudesOfLayers;
    }

    public void SetInformations(Info info)
    {
        this.info.numberOfVerticesPerLayer = info.numberOfVerticesPerLayer;
        this.info.numberOfLayers = info.numberOfLayers;
        this.info.hightOfOneLayer = info.hightOfOneLayer;
        this.info.widthOfCylinder = info.widthOfCylinder;
        this.info.midpointHeightDifference = info.midpointHeightDifference;
        this.info.magnitudesOfLayers = info.magnitudesOfLayers;
    }
}
