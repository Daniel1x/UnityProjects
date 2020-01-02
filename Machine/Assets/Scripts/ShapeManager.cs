using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ShapeManager : MonoBehaviour
{
    /// <summary>
    /// ID number of new created mesh.
    /// </summary>
    public int newSchapeID = 1;
    /// <summary>
    /// Prefix of data file name.
    /// </summary>
    public string dataFileNameStyle = "meshData_shape";
    /// <summary>
    /// Format of created data files.
    /// </summary>
    public string dataFileFormat = ".txt";

    /// <summary>
    /// Mesh generator reference.
    /// </summary>
    private MeshGenerator meshGenerator;
    /// <summary>
    /// Application data files path.
    /// </summary>
    private string path;

    private void Start()
    {
        meshGenerator = FindObjectOfType<MeshGenerator>();
        path = Application.dataPath + "/";
        newSchapeID = 1;
        CheckForNextFreeID();
    }

    /// <summary>
    /// Checking for free ID number.
    /// </summary>
    private void CheckForNextFreeID()
    {
        while (File.Exists(path + dataFileNameStyle + newSchapeID + dataFileFormat))
        {
            newSchapeID++;
        }
    }

    public void SaveNewShapeToFile()
    {
        Debug.Log("Saved!");
    }
}
