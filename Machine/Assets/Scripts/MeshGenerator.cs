﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    /// <summary>
    /// Material placed on the mesh.
    /// </summary>
    public Material material;
    /// <summary>
    /// Material used to show target mesh.
    /// </summary>
    public Material targetMaterial;
    /// <summary>
    /// Name of created mesh.
    /// </summary>
    public string meshName = "GenericMesh";
    /// <summary>
    /// Number of vertices on the circle in one part.
    /// </summary>
    [Range(3, 1024)] public int numVertices = 3;
    /// <summary>
    /// Number of parts from which mesh is created.
    /// </summary>
    [Range(2, 8192)] public int numParts = 2;
    /// <summary>
    /// Hight of one part of the mesh.
    /// </summary>
    [Range(0.0001f, 10f)] public float length = 0.1f;
    /// <summary>
    /// Width of created cylinder.
    /// </summary>
    [Range(0.001f, 10f)] public float width = 1f;
    /// <summary>
    /// The difference in height of the center point with its corresponding layers at the bottom or top of the mesh.
    /// </summary>
    [Range(0.001f, 1f)] public float spikeSize = 0.1f;
    /// <summary>
    /// Table of created verteces.
    /// </summary>
    public Vector3[] vertices;
    /// <summary>
    /// Table of triangles of the mesh.
    /// </summary>
    public int[] triangles;
    /// <summary>
    /// Chosen part which will be modified.
    /// </summary>
    public int layer = 0;
    /// <summary>
    /// Chosen number of parts which will be modified. Starting with chosen layer.
    /// </summary>
    public int layers = 1;
    /// <summary>
    /// Debug drawing lines enabled.
    /// </summary>
    public bool drawVertices = false;
    /// <summary>
    /// Checking numbers of allocated and used memory for mesh.
    /// </summary>
    public bool debugMemory = false;
    /// <summary>
    /// Enable modification of shape, with inputs from keyboard. (Arrows & W-S keys).
    /// </summary>
    public bool enableShapeModification = false;
    /// <summary>
    /// Calculated error between two meshes.
    /// </summary>
    public float calculatedRMSE = float.MaxValue;
    /// <summary>
    /// Text box used to show RMS error.
    /// </summary>
    public TextMesh textBox;

    /// <summary>
    /// Unchanged mesh.
    /// </summary>
    private Vector3[] startVertices;
    /// <summary>
    /// Unchanged mesh.
    /// </summary>
    private Vector3[] targetVertices;
    /// <summary>
    /// Created mesh.
    /// </summary>
    private Mesh mesh;
    /// <summary>
    /// Created mesh collider.
    /// </summary>
    private MeshCollider meshCollider = new MeshCollider();
    /// <summary>
    /// Minimal index of changed vertex.
    /// </summary>
    private int minVertexIndex = int.MaxValue;
    /// <summary>
    /// Maximal index of changed vertex.
    /// </summary>
    private int maxVertexIndex = int.MinValue;
    /// <summary>
    /// Bool that controlls updating mesh.
    /// </summary>
    private bool needToUpdateMesh = true;

    private void Start()
    {
        // Setting knife boundaries.
        KnifeControll.minY = -0.5f;
        KnifeControll.maxY = (numParts * length) + 0.5f;
        // Setting up material.
        GetComponent<MeshRenderer>().material = material;
        // Getting mesh collider.
        meshCollider = GetComponent<MeshCollider>();
        // Creating new mesh.
        mesh = new Mesh();
        // Setting name of mesh.
        mesh.name = meshName;
        // Setting created mesh as mesh filter component.
        GetComponent<MeshFilter>().mesh = mesh;
        // Creating mesh shape.
        CreateShape();
        // Updating mesh with created vertices and triangles.
        mesh.MarkDynamic();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        // Updating mesh collider.
        meshCollider.sharedMesh = mesh;
        meshCollider.sharedMesh.MarkDynamic();
        // Loading target mesh.
        LoadTargetMeshFromFile();
        // Show target.
        InstantiateTargetMesh();
    }

    private void Update()
    {
        if (enableShapeModification && Input.anyKey)
        {
            ResetIndexLimits();
            ModifyShape();
        }

        if (drawVertices) DrawVertices();

        CheckEdges();
        ModifyShapeBasedOnFiles();
        UpdateMesh();

        TryToCalculateRMSE();
    }

    /// <summary>
    /// Creating vertices and triangles of mesh.
    /// </summary>
    private void CreateShape()
    {
        // Allocating memory for vertices table.
        vertices = new Vector3[numVertices * numParts + 2];
        // Allocating memory for traingles table.
        triangles = new int[6 * numVertices * numParts];
        // Creating positions of vertices.
        for (int part = 0; part < numParts; part++)
        {
            for (int vertex = 0; vertex < numVertices; vertex++)
            {
                vertices[vertex + part * numVertices] = Quaternion.Euler(0f, vertex * 360f / numVertices, 0f) * new Vector3(0.5f * width, part * length, 0f);
            }
        }
        // Bottom and top vertices index placeholder.
        int bottomVertex = numVertices * numParts;
        // Bottom vertex.
        vertices[bottomVertex] = Vector3.down * spikeSize;
        // Top vertex.
        vertices[bottomVertex + 1] = new Vector3(0f, vertices[(numVertices * numParts) - 1].y + spikeSize, 0f);
        // Copy created array.
        CopyVerticesArray();
        
        // Triangle index placeholder.
        int triangleID = 0;
        for (int part = 0; part < numParts; part++)
        {
            // Index placeholders for current part.
            int previousPartID = (part - 1) * numVertices;
            int thisPartID = part * numVertices;
            int nextPartID = (part + 1) * numVertices;
            
            if(part==0)// First layer.
            {
                for (int vertex = 0; vertex < numVertices; vertex++)
                {
                    triangles[triangleID++] = thisPartID + vertex;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numVertices;
                    triangles[triangleID++] = nextPartID + (vertex + 1) % numVertices;
                }
            }
            else if (part == numParts - 1)// Last layer.
            {
                for (int vertex = 0; vertex < numVertices; vertex++)
                {
                    triangles[triangleID++] = previousPartID + vertex % numVertices;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numVertices;
                    triangles[triangleID++] = thisPartID + vertex;
                }
            }
            else // Middle layers.
            {
                for (int vertex = 0; vertex < numVertices; vertex++)
                {
                    triangles[triangleID++] = thisPartID + vertex;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numVertices;
                    triangles[triangleID++] = nextPartID + (vertex + 1) % numVertices;
                }
                for (int vertex = 0; vertex < numVertices; vertex++)
                {
                    triangles[triangleID++] = previousPartID + vertex % numVertices;
                    triangles[triangleID++] = thisPartID + (vertex + 1) % numVertices;
                    triangles[triangleID++] = thisPartID + vertex;
                }
            }
        }

        int topVertex = numVertices * (numParts - 1);
        for (int i = 0; i < numVertices; i++)
        {
            //Down
            triangles[triangleID++] = bottomVertex;
            triangles[triangleID++] = (i + 1) % numVertices;
            triangles[triangleID++] = i;
            //Up
            triangles[triangleID++] = bottomVertex + 1;
            triangles[triangleID++] = topVertex + i;
            triangles[triangleID++] = topVertex + (i + 1) % numVertices;
        }

        if (debugMemory)
        {
            // Debug: Number of allocated space.
            Debug.Log("Memory located: " + triangles.Length);
            // Debug: Number of used space.
            Debug.Log("Memory used: " + triangleID);
            Debug.Log("Memory allocated and used should be equal!");
        }
    }

    /// <summary>
    /// Updating mesh and collider with modyfied vertices. Recalculating mesh normals.
    /// </summary>
    private void UpdateMesh()
    {
        if (!needToUpdateMesh) return;
        mesh.SetVertices(vertices);
        meshCollider.sharedMesh = mesh;
        mesh.RecalculateNormals();
        if (debugMemory) Debug.Log("Mesh Updated!");
        needToUpdateMesh = false;
    }
    
    /// <summary>
    /// Changing shape of mesh via inputs from keyboard. W and S keys changes how many layers of mesh are changing at the same time. 
    /// Vertical arrows changes actualy chosen layer. Horizontal arrows changes thickness of all chosen layers.
    /// </summary>
    private void ModifyShape()
    {
        // Collecting and clamping input values.
        if (Input.GetKey(KeyCode.W)) layers++;
        if (Input.GetKey(KeyCode.S)) layers--;
        layers = Mathf.Clamp(layers, 1, numParts);
        if (Input.GetKey(KeyCode.UpArrow)) layer++;
        if (Input.GetKey(KeyCode.DownArrow)) layer--;
        layer = Mathf.Clamp(layer, 0, numParts - layers);
        float input = Input.GetAxis("Horizontal") * Time.deltaTime;
        // Calculating offset of actually chosen layer.
        int globalOffset = layer * numVertices;
        if (globalOffset >= vertices.Length || globalOffset < 0) return;
        // Changing magnitude of all chosen vertices.
        for (int j = 0; j < layers; j++)
        {
            int offset = globalOffset + j * numVertices;
            Vector3 magVect = vertices[offset];
            magVect.y = 0;
            float magnitude = Mathf.Clamp(magVect.magnitude + input, 0.0001f, 100f);

            for (int i = 0; i < numVertices; i++)
            {
                Vector3 vertex = vertices[i + offset];
                float hight = vertex.y;
                vertex.y = 0f;
                vertices[i + offset] = vertex.normalized * magnitude + Vector3.up * hight;
                CheckIndex(i + offset);
            }
        }
        needToUpdateMesh = true;
    }

    /// <summary>
    /// Checking for collision of knife edges with created mesh.
    /// </summary>
    private void CheckEdges()
    {
        for (int i = 0; i < Knife.numberOfPoints; i++)
        {
            // Projecting raycasts from knife edges.
            if (Physics.Raycast(Knife.edges[i], Knife.cutDir, out RaycastHit hit, Knife.rayRange))
            {
                if (!hit.transform.CompareTag("GeneratedMesh")) break;
                // Index of hitted triangle of mesh.
                int triangleID = hit.triangleIndex;
                // Position of hit point.
                Vector3 hitPoint = hit.point;
                hitPoint.y = 0;
                // Distance from hit position to Y axis.
                float hitPointMagnitude = Vector3.Distance(hitPoint, Vector3.zero) - (Knife.rayRange - hit.distance);
                for (int j = 0; j < 3; j++)
                {
                    // Position of vertex of referenced triangle.
                    Vector3 point = vertices[triangles[(3 * triangleID) + j]];
                    // Height value of vertex position.
                    float height = point.y;
                    point.y = 0f;
                    // Distance from vertex to Y axis.
                    float pointMagnitude = Vector3.Distance(point, Vector3.zero);
                    // Checking distance differences.
                    if (pointMagnitude > hitPointMagnitude) pointMagnitude = hitPointMagnitude;
                    // Creating new position values of vertex.
                    vertices[triangles[(3 * triangleID) + j]] = (point.normalized * pointMagnitude) + (height * Vector3.up);
                }
                needToUpdateMesh = true;
                KnifeControll.needToCheckShape = true;
            }
        }
    }

    /// <summary>
    /// Reseting values of minimal and maximal ID of changed vertex.
    /// </summary>
    private void ResetIndexLimits()
    {
        minVertexIndex = vertices.Length;
        maxVertexIndex = 0;
    }

    /// <summary>
    /// Checking if the current index has exceeded its limits.
    /// </summary>
    /// <param name="actualID">ID of actual vertex.</param>
    private void CheckIndex(int actualID)
    {
        if (actualID < minVertexIndex) minVertexIndex = actualID;
        if (actualID > maxVertexIndex) maxVertexIndex = actualID;
    }
    
    /// <summary>
    /// Printing values of limits.
    /// </summary>
    private void PrintIndexLimits()
    {
        Debug.Log("Min: " + minVertexIndex);
        Debug.Log("Max: " + maxVertexIndex);
    }

    /// <summary>
    /// Drawing lines of all triangles in mesh.
    /// </summary>
    private void DrawVertices()
    {
        for (int i = 0; i < triangles.Length / 3; i++)
        {
            Debug.DrawLine(transform.rotation * vertices[triangles[3 * i]], transform.rotation * vertices[triangles[3 * i + 1]], Color.red);
            Debug.DrawLine(transform.rotation * vertices[triangles[3 * i + 1]], transform.rotation * vertices[triangles[3 * i + 2]], Color.red);
            Debug.DrawLine(transform.rotation * vertices[triangles[3 * i + 2]], transform.rotation * vertices[triangles[3 * i]], Color.red);
        }
    }

    // Collisions
    private void OnCollisionEnter(Collision collision) => Debug.Log("Collision Enter");
    private void OnCollisionStay(Collision collision) => Debug.Log("Collision Stay");
    private void OnCollisionExit(Collision collision) => Debug.Log("Collision Exit");

    /// <summary>
    /// Save mesh vertices to created txt file.
    /// </summary>
    private void SaveMeshToFile()
    {
        string path = Application.dataPath + "/meshData.txt";
        StreamWriter dataFile = File.CreateText(path);
        
        for(int i = 0; i < vertices.Length; i++)
        {
            string line = "";
            line += vertices[i].x.ToString() + ";";
            line += vertices[i].y.ToString() + ";";
            line += vertices[i].z.ToString() + ";";

            dataFile.WriteLine(line);
        }
        dataFile.Close();
    }

    /// <summary>
    /// Load mesh vertices from txt file.
    /// </summary>
    private void LoadMeshFromFile()
    {
        string path = Application.dataPath + "/meshData.txt";
        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader dataFile = File.OpenText(path);
            dataFile.BaseStream.Position = 0;
            for(int i = 0; i < lineCount; i++)
            {
                string line = dataFile.ReadLine();
                string[] data = line.Split(';');
                vertices[i] = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
            }
            dataFile.Close();
        }
        needToUpdateMesh = true;
    }

    /// <summary>
    /// Reset vertices positions.
    /// </summary>
    private void ResetMesh()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = startVertices[i];
        }
        needToUpdateMesh = true;
    }

    /// <summary>
    /// Copying vertices values.
    /// </summary>
    private void CopyVerticesArray()
    {
        startVertices = new Vector3[vertices.Length];
        for(int i = 0; i < vertices.Length; i++)
        {
            startVertices[i] = vertices[i];
        }
    }

    /// <summary>
    /// The function supports invisible buttons at the top of the screen (Higher than 0.8 screen height). 
    /// The mesh is saved to a file when a point is pressed in one third of the screen width (from the left). 
    /// The mesh is loaded from the file when the point is pressed in the center (between 1/3 and 2/3 of the screen width). 
    /// The mesh shape is reset when the point is pressed in one third of the screen width (from the right).
    /// </summary>
    private void ModifyShapeBasedOnFiles()
    {
        if (!Input.anyKey) return;
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y / Screen.height >= 0.8f)
        {
            float mousePos = Input.mousePosition.x / Screen.width;
            if (mousePos < 0.333f) SaveMeshToFile();
            else if (mousePos >= 0.333f && mousePos < 0.666f) LoadMeshFromFile();
            else if (mousePos >= 0.666f) ResetMesh();
        }
    }

    /// <summary>
    /// Calculating RMSE between two meshes. 
    /// Error is based on magnitudes of vertices with respect to Y axis.
    /// </summary>
    /// <param name="meshVertices">Actual mesh.</param>
    /// <param name="targetVertices">Target mesh.</param>
    private void CalculateRMSE(Vector3[] meshVertices, Vector3[] targetVertices)
    {
        int numVertices = meshVertices.Length;
        float error = 0;
        for(int i = 0; i < numVertices; i++)
        {
            Vector3 v1 = meshVertices[i];
            v1.y = 0;
            Vector3 v2 = targetVertices[i];
            v2.y = 0;
            float m1 = v1.magnitude;
            float m2 = v2.magnitude;
            error += Mathf.Pow((m1 - m2) * 10f, 2f);
        }
        calculatedRMSE = error / numVertices;
        textBox.text = calculatedRMSE.ToString("0.000");
        KnifeControll.needToCheckShape = false;
    }

    /// <summary>
    /// Function that tries to calculate RMSE.
    /// </summary>
    private void TryToCalculateRMSE()
    {
        if (KnifeControll.checkingAvailable && KnifeControll.needToCheckShape)
        {
            CalculateRMSE(vertices, targetVertices);
        }
    }

    /// <summary>
    /// Loading from file positions of target vertices.
    /// </summary>
    private void LoadTargetMeshFromFile(string shapeNr = "1")
    {
        string path = Application.dataPath + "/meshData_shape" + shapeNr + ".txt";
        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            targetVertices = new Vector3[lineCount];
            StreamReader dataFile = File.OpenText(path);
            dataFile.BaseStream.Position = 0;
            for (int i = 0; i < lineCount; i++)
            {
                string line = dataFile.ReadLine();
                string[] data = line.Split(';');
                targetVertices[i] = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
            }
            dataFile.Close();
        }
    }

    /// <summary>
    /// Creating new gameobject from target mesh.
    /// </summary>
    private void InstantiateTargetMesh()
    {
        GameObject target = new GameObject();
        target.name = "Target";
        target.AddComponent<MeshFilter>();
        target.AddComponent<MeshRenderer>();
        target.GetComponent<MeshRenderer>().material = targetMaterial;
        Mesh targetMesh = new Mesh();
        targetMesh.SetVertices(targetVertices);
        targetMesh.SetTriangles(triangles, 0);
        targetMesh.RecalculateNormals();
        targetMesh.name = "TargetMesh";
        target.GetComponent<MeshFilter>().mesh = targetMesh;
    }
}
