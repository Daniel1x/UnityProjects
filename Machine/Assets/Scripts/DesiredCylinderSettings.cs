using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CylinderSettings", menuName = "Create Machine Part/Cylinder Settings", order = 1)]
public class DesiredCylinderSettings : ScriptableObject
{
    public Material material;
    public string gameObjectName = "GenericCylinder";
    public string gameObjectTag = "GeneratedCylinder";
    public Vector3 spawnPosition = new Vector3();
    public Vector3 spawnRotationEuler = new Vector3();
    public int numberOfVerticesPerLayer = 12;
    public int numberOfLayers = 10;
    public float hightOfOneLayer = 0.1f;
    public float widthOfCylinder = 1f;
    public float midpointHeightDifference = 0.1f;

    public void SetDefaults()
    {
        gameObjectName = "GenericCylinder";
        gameObjectTag = "GeneratedCylinder";
        spawnPosition = new Vector3();
        spawnRotationEuler = new Vector3();
        numberOfVerticesPerLayer = 12;
        numberOfLayers = 10;
        hightOfOneLayer = 0.1f;
        widthOfCylinder = 1f;
        midpointHeightDifference = 0.1f;
    }
}
