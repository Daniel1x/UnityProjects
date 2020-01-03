using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ShapeManager : MonoBehaviour
{
    /// <summary>
    /// Scriptable object used to save data.
    /// </summary>
    public MeshDataContainer meshDataContainer;
    /// <summary>
    /// Scriptable object from which data will be copied.
    /// </summary>
    public MeshDataContainer dataFrom;
    /// <summary>
    /// Scriptable object to which data will be copied.
    /// </summary>
    public MeshDataContainer dataTo;
    /// <summary>
    /// Scriptable obiect from which data will be copied to target mesh game object.
    /// </summary>
    public MeshDataContainer targetMesh;

    /// <summary>
    /// Mesh generator reference.
    /// </summary>
    private MeshGenerator meshGenerator;

    private void Start()
    {
        meshGenerator = FindObjectOfType<MeshGenerator>();
    }

    /// <summary>
    /// Collecting mesh data for a scriptable object.
    /// </summary>
    public void SaveNewShapeAsScriptableObject()
    {
        meshDataContainer.numVertices = meshGenerator.numVertices;
        meshDataContainer.numParts = meshGenerator.numParts;
        meshDataContainer.length = meshGenerator.length;
        meshDataContainer.width = meshGenerator.width;
        meshDataContainer.spikeSize = meshGenerator.spikeSize;
        meshDataContainer.vertices = CopyTable(meshGenerator.vertices);
        meshDataContainer.triangles = CopyTable(meshGenerator.triangles);
    }

    /// <summary>
    /// Loading mesh data from a scriptable object.
    /// </summary>
    public void LoadShapeFromScriptableObject()
    {
        meshGenerator.numVertices = meshDataContainer.numVertices;
        meshGenerator.numParts = meshDataContainer.numParts;
        meshGenerator.length = meshDataContainer.length;
        meshGenerator.width = meshDataContainer.width;
        meshGenerator.spikeSize = meshDataContainer.spikeSize;
        meshGenerator.vertices = CopyTable(meshDataContainer.vertices);
        meshGenerator.triangles = CopyTable(meshDataContainer.triangles);

        meshGenerator.ForceUpdateMesh();
    }

    /// <summary>
    /// Duplicating an array.
    /// </summary>
    /// <param name="original">Original array.</param>
    /// <returns></returns>
    private Vector3[] CopyTable(Vector3[] original)
    {
        Vector3[] output = new Vector3[original.Length];
        for (int i = 0; i < original.Length; i++) output[i] = original[i];
        return output;
    }

    /// <summary>
    /// Duplicating an array.
    /// </summary>
    /// <param name="original">Original array.</param>
    /// <returns></returns>
    private int[] CopyTable(int[] original)
    {
        int[] output = new int[original.Length];
        for (int i = 0; i < original.Length; i++) output[i] = original[i];
        return output;
    }

    /// <summary>
    /// Restarting generation of the mesh.
    /// </summary>
    public void RestartMeshGenerator()
    {
        meshGenerator.LoadTargetMesh(targetMesh.vertices);
        meshGenerator.Restart();
    }

    /// <summary>
    /// Copying all data from scriptable object to another.
    /// </summary>
    /// <param name="from">Original scriptable object</param>
    /// <param name="to">Target scriptable object</param>
    public void CopyData(MeshDataContainer from, MeshDataContainer to)
    {
        to.numVertices = from.numVertices;
        to.numParts = from.numParts;
        to.length = from.length;
        to.width = from.width;
        to.spikeSize = from.spikeSize;
        to.vertices = CopyTable(from.vertices);
        to.triangles = CopyTable(from.triangles);
    }

    /// <summary>
    /// Set target shape mesh data.
    /// </summary>
    public void SetTargetShape()
    {
        meshGenerator.LoadTargetMesh(targetMesh.vertices);
    }
}
