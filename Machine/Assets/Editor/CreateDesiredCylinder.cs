using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateDesiredCylinder : ScriptableWizard
{
    public Material material;
    public string gameObjectName = "GenericCylinder";
    public string gameObjectTag = "GeneratedCylinder";
    public Vector3 spawnPosition = new Vector3();
    public Vector3 spawnRotationEuler = new Vector3();
    [Range(3, 36)] public int numberOfVerticesPerLayer = 12;
    [Range(2, 1024)] public int numberOfLayers = 10;
    [Range(0.025f, 10f)] public float hightOfOneLayer = 0.1f;
    [Range(0.025f, 100f)] public float widthOfCylinder = 1f;
    [Range(0f, 10f)] public float midpointHeightDifference = 0.1f;
    [Range(-2f, 2f)] public float rotationSpeed = 0f;
    public bool markMeshAsDynamic = true;
    public bool addRigidbody = false;
    public bool isRigidbodyKinematic = true;
    [Range(1, 100)] public int numberOfCylindersToCreate = 1;

    private const string meshName = "GenericMesh";
    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;
    private MeshCollider meshCollider;
    private static int cylinderNumber = 0;

    [MenuItem("Cylinder Creator/Create Desired Cylinder")]
    static void CreateWizard()
    {
        DisplayWizard<CreateDesiredCylinder>("Create Cylinder", "Create New Cylinder");
    }

    private void Awake()
    {
        isValid = false;
        helpString = "Remember to choose the material!";
        material = FindMaterialWithName("Metal");

        GetLastSettings();
    }

    private void OnWizardCreate()
    {
        for (int i = 0; i < numberOfCylindersToCreate; i++)
            CreateOneCylinder();

        SetLastSettings();
    }

    private void CreateOneCylinder()
    {
        GameObject cylinderGO = new GameObject();
        cylinderGO.name = gameObjectName + cylinderNumber.ToString();
        cylinderGO.tag = gameObjectTag;
        cylinderNumber++;

        cylinderGO.transform.position = spawnPosition;
        cylinderGO.transform.rotation = Quaternion.Euler(spawnRotationEuler);

        MeshFilter meshFilter = cylinderGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cylinderGO.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = cylinderGO.AddComponent<MeshCollider>();
        GenericMeshInfo genericMeshInfo = cylinderGO.AddComponent<GenericMeshInfo>();
        genericMeshInfo.SetInformations(numberOfVerticesPerLayer, numberOfLayers, hightOfOneLayer, widthOfCylinder, midpointHeightDifference);

        if (rotationSpeed != 0)
        {
            RotateMetal rotateMetal = cylinderGO.AddComponent<RotateMetal>();
            rotateMetal.rotationsPerSecond = rotationSpeed;
        }

        if (addRigidbody)
        {
            Rigidbody rigidbody = cylinderGO.AddComponent<Rigidbody>();
            rigidbody.isKinematic = isRigidbodyKinematic;
            if (!isRigidbodyKinematic) meshCollider.convex = true;
        }

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
        triangles = new int[6 * numberOfVerticesPerLayer * numberOfLayers];
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

    private Material FindMaterialWithName(string name)
    {
        Material material = null;
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        for (int i = 0; i < materials.Length; i++)
            if (materials[i].name == "Metal") material = materials[i];
        return material;
    }

    private DesiredCylinderSettings FindLastCylinderSettings(string lastCylinderSettingsFileName)
    {
        DesiredCylinderSettings lastCylinderSettings = null;
        DesiredCylinderSettings[] cylinderSettings = Resources.FindObjectsOfTypeAll<DesiredCylinderSettings>();
        for (int i = 0; i < cylinderSettings.Length; i++)
            if (cylinderSettings[i].name == lastCylinderSettingsFileName) lastCylinderSettings = cylinderSettings[i];
        return lastCylinderSettings;
    }
    
    private void SetLastSettings()
    {
        DesiredCylinderSettings lastCylinderSettings = FindLastCylinderSettings("LastCylinderSettings");
        if (lastCylinderSettings == null) return;
        lastCylinderSettings.gameObjectName = gameObjectName;
        lastCylinderSettings.gameObjectTag = gameObjectTag;
        lastCylinderSettings.spawnPosition = spawnPosition;
        lastCylinderSettings.spawnRotationEuler = spawnRotationEuler;
        lastCylinderSettings.numberOfVerticesPerLayer = numberOfVerticesPerLayer;
        lastCylinderSettings.numberOfLayers = numberOfLayers;
        lastCylinderSettings.hightOfOneLayer = hightOfOneLayer;
        lastCylinderSettings.widthOfCylinder = widthOfCylinder;
        lastCylinderSettings.midpointHeightDifference = midpointHeightDifference;
        lastCylinderSettings.rotationSpeed = rotationSpeed;
        lastCylinderSettings.markMeshAsDynamic = markMeshAsDynamic;
        lastCylinderSettings.addRigidbody = addRigidbody;
        lastCylinderSettings.isRigidbodyKinematic = isRigidbodyKinematic;
    }

    private void GetLastSettings()
    {
        DesiredCylinderSettings lastCylinderSettings = FindLastCylinderSettings("LastCylinderSettings");
        if (lastCylinderSettings == null) return;
        gameObjectName = lastCylinderSettings.gameObjectName;
        gameObjectTag = lastCylinderSettings.gameObjectTag;
        spawnPosition = lastCylinderSettings.spawnPosition;
        spawnRotationEuler = lastCylinderSettings.spawnRotationEuler;
        numberOfVerticesPerLayer = lastCylinderSettings.numberOfVerticesPerLayer;
        numberOfLayers = lastCylinderSettings.numberOfLayers;
        hightOfOneLayer = lastCylinderSettings.hightOfOneLayer;
        widthOfCylinder = lastCylinderSettings.widthOfCylinder;
        midpointHeightDifference = lastCylinderSettings.midpointHeightDifference;
        rotationSpeed = lastCylinderSettings.rotationSpeed;
        markMeshAsDynamic = lastCylinderSettings.markMeshAsDynamic;
        addRigidbody = lastCylinderSettings.addRigidbody;
        isRigidbodyKinematic = lastCylinderSettings.isRigidbodyKinematic;

    }

}
