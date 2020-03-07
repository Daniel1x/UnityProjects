using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeshDataContainer : ScriptableObject
{
    [Header("Knife Settings")]
    public float moveSpeed;
    public float downSpeed;
    public KnifeControll.MoveStyle moveStyle;

    [Header("Mesh Settings")]
    public float rotationFrequency;
    public int numVertices;
    public int numParts;
    public float length;
    public float width;
    public float spikeSize;
    public Vector3[] vertices;
    public int[] triangles;
}
