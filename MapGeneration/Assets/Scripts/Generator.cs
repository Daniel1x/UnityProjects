using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Generator : MonoBehaviour
{
    [Range(2, 255)] public int numVerticesOnXAxis = 2;
    [Range(2, 255)] public int numVerticesOnZAxis = 2;
    public float density = 1f;
    public float amplitudeX = 1f;
    public float amplitudeZ = 1f;
    public float frequencyX = 1f;
    public float frequencyZ = 1f;
    public Material material;

    private Vector3[] vertices;
    private int[] triangles;

    private Mesh mesh;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        SetUpVertices();
        SetUpTriangles();
        SetUpHightOfVertices(amplitudeX, amplitudeZ, frequencyX, frequencyZ);
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }

    private void SetUpVertices()
    {
        float distance = 1f / density;
        vertices = new Vector3[numVerticesOnXAxis * numVerticesOnZAxis];
        for(int i = 0; i < numVerticesOnXAxis; i++)
        {
            for(int j = 0; j < numVerticesOnZAxis; j++)
            {
                vertices[(i * numVerticesOnXAxis) + j] = new Vector3(i * distance, 0f, j * distance);
            }
        }
    }

    private void SetUpHightOfVertices()
    {
        float hight = 0f;
        for (int i = 0; i < numVerticesOnXAxis; i++)
        {
            for (int j = 0; j < numVerticesOnZAxis; j++)
            {
                if (i == 0)
                {
                    hight += UnityEngine.Random.Range(-0.1f, 0.1f);
                    vertices[(i * numVerticesOnXAxis) + j].y = hight;
                }
                else
                {
                    hight = UnityEngine.Random.Range(-0.1f, 0.1f);
                    vertices[(i * numVerticesOnXAxis) + j].y = vertices[((i - 1) * numVerticesOnXAxis) + j].y + hight;
                }
            }
        }
    }

    private void SetUpHightOfVertices(float amplitudeX, float amplitudeZ, float frequencyX, float frequencyZ)
    {
        for (int i = 0; i < numVerticesOnXAxis; i++)
        {
            for (int j = 0; j < numVerticesOnZAxis; j++)
            {
                Vector3 vert = vertices[(i * numVerticesOnXAxis) + j];
                float hight = amplitudeX * Mathf.Sin(frequencyX * vert.x) + amplitudeZ * Mathf.Sin(frequencyZ * vert.z);
                vertices[(i * numVerticesOnXAxis) + j].y = hight;
            }
        }
    }

    private void SetUpTriangles()
    {
        int triangleID = 0;
        triangles = new int[6 * (numVerticesOnXAxis - 1) * (numVerticesOnZAxis - 1)];
        for(int i = 0; i < numVerticesOnXAxis - 1; i++)
        {
            for (int j = 0; j < numVerticesOnZAxis - 1; j++)
            {
                triangles[triangleID++] = (i * numVerticesOnXAxis) + j;
                triangles[triangleID++] = (i * numVerticesOnXAxis) + j + 1;
                triangles[triangleID++] = ((i + 1) * numVerticesOnXAxis) + j + 1;

                triangles[triangleID++] = ((i + 1) * numVerticesOnXAxis) + j + 1;
                triangles[triangleID++] = ((i + 1) * numVerticesOnXAxis) + j;
                triangles[triangleID++] = (i * numVerticesOnXAxis) + j;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.transform.name);
        Debug.Log(collision.contacts[0].thisCollider.transform.name);
    }
}
