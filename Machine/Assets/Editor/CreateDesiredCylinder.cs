using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateDesiredCylinder : ScriptableWizard
{
    public Material material;
    public string gameObjectName = "GenericCylinder";
    [Range(3, 36)] public int numberOfVerticesPerLayer = 5;
    [Range(2, 1024)] public int numberOfLayers = 3;
    [Range(0.0001f, 10f)] public float hightOfOneLayer = 0.1f;
    [Range(0.01f, 10f)] public float widthOfCylinder = 1f;
    [Range(0.001f, 1f)] public float midpointHeightDifference = 0.1f;
    public bool markMeshAsDynamic = true;

    private const string meshName = "GenericMesh";
    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;
    private MeshCollider meshCollider;

    [MenuItem("Cylinder Creator/Create Desired Cylinder")]
    static void CreateWizard()
    {
        DisplayWizard<CreateDesiredCylinder>("Create Cylinder", "Create New Cylinder");
    }

    private void Awake()
    {
        isValid = false;
        helpString = "Remember to choose the material!";
    }

    private void OnWizardCreate()
    {
        GameObject cylinderGO = new GameObject();
        cylinderGO.name = gameObjectName;

        MeshFilter meshFilter = cylinderGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cylinderGO.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = cylinderGO.AddComponent<MeshCollider>();

        if (material) meshRenderer.material = material;

        mesh = new Mesh();
        mesh.name = meshName;
        meshFilter.mesh = mesh;

        CreateDesiredMesh();

        meshCollider.sharedMesh = mesh;
        if (markMeshAsDynamic) meshCollider.sharedMesh.MarkDynamic();
    }

    private void CreateDesiredMesh()
    {
        CreateVerticesAndTriangles();
        if (markMeshAsDynamic) mesh.MarkDynamic();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
    }

    private void CreateVerticesAndTriangles()
    {
        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices()
    {
        vertices = new Vector3[numberOfVerticesPerLayer * numberOfLayers + 2];
        triangles = new int[6 * numberOfVerticesPerLayer * numberOfLayers];
        for (int layer = 0; layer < numberOfLayers; layer++)
        {
            for (int vertex = 0; vertex < numberOfVerticesPerLayer; vertex++)
            {
                vertices[vertex + layer * numberOfVerticesPerLayer] =
                    Quaternion.Euler(0f, 360f * vertex / numberOfVerticesPerLayer, 0f) 
                    * new Vector3(0.5f * widthOfCylinder, layer * hightOfOneLayer, 0f);
            }
        }
        /// Bottom and top vertices index placeholder.
        int bottomVertex = numberOfVerticesPerLayer * numberOfLayers;
        vertices[bottomVertex] = Vector3.down * midpointHeightDifference;
        vertices[bottomVertex + 1] = new Vector3(0f, 
            vertices[(numberOfVerticesPerLayer * numberOfLayers) - 1].y + midpointHeightDifference, 0f);
    }

    private void CreateTriangles()
    {
        int triangleID = 0;
        for (int part = 0; part < numberOfLayers; part++)
        {
            // Index placeholders for current part.
            int previousPartID = (part - 1) * numberOfVerticesPerLayer;
            int thisPartID = part * numberOfVerticesPerLayer;
            int nextPartID = (part + 1) * numberOfVerticesPerLayer;

            if (part == 0)// First layer.
            {
                for (int vertex = 0; vertex < numberOfVerticesPerLayer; vertex++)
                {
                    triangles[triangleID++] = thisPartID + vertex;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numberOfVerticesPerLayer;
                    triangles[triangleID++] = nextPartID + (vertex + 1) % numberOfVerticesPerLayer;
                }
            }
            else if (part == numberOfLayers - 1)// Last layer.
            {
                for (int vertex = 0; vertex < numberOfVerticesPerLayer; vertex++)
                {
                    triangles[triangleID++] = previousPartID + vertex % numberOfVerticesPerLayer;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numberOfVerticesPerLayer;
                    triangles[triangleID++] = thisPartID + vertex;
                }
            }
            else // Middle layers.
            {
                for (int vertex = 0; vertex < numberOfVerticesPerLayer; vertex++)
                {
                    triangles[triangleID++] = thisPartID + vertex;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numberOfVerticesPerLayer;
                    triangles[triangleID++] = nextPartID + (vertex + 1) % numberOfVerticesPerLayer;
                }
                for (int vertex = 0; vertex < numberOfVerticesPerLayer; vertex++)
                {
                    triangles[triangleID++] = previousPartID + vertex % numberOfVerticesPerLayer;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numberOfVerticesPerLayer;
                    triangles[triangleID++] = thisPartID + vertex;
                }
            }
        }

        // Bottom and top vertices index placeholder.
        int bottomVertex = numberOfVerticesPerLayer * numberOfLayers;
        int topVertex = numberOfVerticesPerLayer * (numberOfLayers - 1);
        for (int i = 0; i < numberOfVerticesPerLayer; i++)
        {
            //Down
            triangles[triangleID++] = bottomVertex;
            triangles[triangleID++] = (i + 1) % numberOfVerticesPerLayer;
            triangles[triangleID++] = i;
            //Up
            triangles[triangleID++] = bottomVertex + 1;
            triangles[triangleID++] = topVertex + i;
            triangles[triangleID++] = topVertex + (i + 1) % numberOfVerticesPerLayer;
        }
    }

    private void OnWizardUpdate()
    {
        isValid = material ? true : false;
    }
}
