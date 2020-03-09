﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticCylinderCreator
{
    private static int gameObjectID = 0;
    private static string defaultMeshName = "GenericMesh";

    public static GameObject CreateCylinder(Transform parentObject, string gameObjectName, string gameObjectTag, Vector3 spawnPosition, int numberOfVerticesPerLayer,
        int numberOfLayers, float widthOfCylinder, float hightOfOneLayer, float midpointHeightDifference, string materialName)
    {
        GameObject cylinderGO = new GameObject() as GameObject;
        cylinderGO.transform.position = spawnPosition;
        cylinderGO.name = gameObjectName + gameObjectID.ToString();
        cylinderGO.tag = gameObjectTag;
        gameObjectID++;

        if (parentObject != null) cylinderGO.transform.parent = parentObject;

        MeshFilter meshFilter = cylinderGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cylinderGO.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = cylinderGO.AddComponent<MeshCollider>();
        GenericMeshInfo genericMeshInfo = cylinderGO.AddComponent<GenericMeshInfo>();

        genericMeshInfo.SetInformations(numberOfVerticesPerLayer, numberOfLayers, hightOfOneLayer, widthOfCylinder, midpointHeightDifference, CreateMagnitudesArray(numberOfLayers, widthOfCylinder));

        Material material = FindDefaultMaterial();
        if (material != null) meshRenderer.material = material;

        Mesh mesh = new Mesh();
        mesh = CreateMesh(mesh, numberOfVerticesPerLayer, numberOfLayers, widthOfCylinder, hightOfOneLayer, midpointHeightDifference);
        mesh.name = defaultMeshName;
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

        return cylinderGO;
    }

    public static Mesh CreateMesh(Mesh meshToModify, int numberOfVerticesPerLayer, int numberOfLayers, float widthOfCylinder, float hightOfOneLayer, float midpointHeightDifference)
    {
        Mesh mesh = meshToModify;

        Vector3[] vertices = CreateVertices(numberOfVerticesPerLayer, numberOfLayers, widthOfCylinder, hightOfOneLayer, midpointHeightDifference);
        int[] triangles = CreateTriangles(numberOfVerticesPerLayer, numberOfLayers);
        mesh.MarkDynamic();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Vector3[] CreateVertices(int numberOfVerticesPerLayer, int numberOfLayers, float widthOfCylinder, float hightOfOneLayer, float midpointHeightDifference)
    {
        Vector3[] vertices = new Vector3[numberOfVerticesPerLayer * numberOfLayers + 2];
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
        vertices[bottomVertex + 1] = new Vector3(0f, vertices[(numberOfVerticesPerLayer * numberOfLayers) - 1].y + midpointHeightDifference, 0f);
        
        return vertices;
    }

    public static int[] CreateTriangles(int numberOfVerticesPerLayer, int numberOfLayers)
    {
        int[] triangles = new int[6 * numberOfVerticesPerLayer * numberOfLayers];
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

        return triangles;
    }

    public static float[] CreateMagnitudesArray(int numberOfLayers, float widthOfCylinder)
    {
        float[] magnitudesOfLayers = new float[numberOfLayers];

        for (int i = 0; i < magnitudesOfLayers.Length; i++)
            magnitudesOfLayers[i] = 0.5f * widthOfCylinder;

        return magnitudesOfLayers;
    }

    public static Material FindDefaultMaterial(string materialName = "Metal")
    {
        Material material = null;
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();

        for (int i = 0; i < materials.Length; i++)
            if (materials[i].name == materialName) material = materials[i];

        if (material == null) Debug.Log("No default Material found!");

        return material;
    }

}