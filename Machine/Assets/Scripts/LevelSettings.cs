using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "LevelSettings", menuName = "Create Machine Part/Level Settings", order = 2)]
public class LevelSettings : ScriptableObject
{
    public Info[] meshInfoArray = null;
    public float[] cylinderPositionHeights = new float[0];

    public void SetLevelInformations(DesiredCylinderSettings[] cylinderSettings)
    {
        int numberOfCylinders = cylinderSettings.Length;
        cylinderPositionHeights = new float[numberOfCylinders];
        meshInfoArray = new Info[numberOfCylinders];
    }

    public void SetMeshInfoArray(GenericMeshInfo[] genericMeshInfo)
    {
        int arrayLength = genericMeshInfo.Length;
        meshInfoArray = new Info[arrayLength];
        for (int index = 0; index < arrayLength; index++)
        {
            Info meshInfo = genericMeshInfo[index].info;
            meshInfoArray[index] = meshInfo;
        }
    }
}
