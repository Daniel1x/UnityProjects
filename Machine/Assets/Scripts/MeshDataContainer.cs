using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeshDataContainer : ScriptableObject
{
    public int numVertices;
    public int numParts;
    public float length;
    public float width;
    public float spikeSize;
    public Vector3[] vertices;
    public int[] triangles;
}
