using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [Range(3, 36)] public int numVertices = 3;
    /// <summary>
    /// Number of parts from which mesh is created.
    /// </summary>
    [Range(2, 1024)] public int numParts = 2;
    /// <summary>
    /// Hight of one part of the mesh.
    /// </summary>
    [Range(0.0001f, 10f)] public float length = 0.1f;
    /// <summary>
    /// Width of created cylinder.
    /// </summary>
    [Range(0.01f, 10f)] public float width = 1f;
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
    public Text textBox;
    
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
    /// <summary>
    /// Reference to instatiated target mesh game object.
    /// </summary>
    private GameObject targetMeshGO;
    /// <summary>
    /// Variable that indicates if target mesh was loaded.
    /// </summary>
    private bool targetLoaded = false;

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
    }

    public void Restart()
    {
        Destroy(targetMeshGO);
        targetLoaded = false;
        mesh.Clear();
        Start();
    }

    private void Update()
    {
        if (enableShapeModification && Input.anyKey)
        {
            ResetIndexLimits();
            ModifyShape();
        }
        CheckEdges();
        TryToUpdateMesh();
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
    }

    /// <summary>
    /// Public function used to force mesh update.
    /// </summary>
    public void ForceUpdateMesh()
    {
        needToUpdateMesh = true;
        TryToUpdateMesh();
    }

    /// <summary>
    /// Updating mesh and collider with modyfied vertices. Recalculating mesh normals.
    /// </summary>
    private void TryToUpdateMesh()
    {
        if (!needToUpdateMesh) return;
        mesh.Clear();                       // <-- maybe without this
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);    // <-- maybe without this
        meshCollider.sharedMesh = mesh;
        mesh.RecalculateNormals();
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
        if (!targetLoaded) return;
        if (KnifeControll.checkingAvailable && KnifeControll.needToCheckShape)
        {
            CalculateRMSE(vertices, targetVertices);
        }
    }

    /// <summary>
    /// Loading from file positions of target vertices.
    /// </summary>
    public void LoadTargetMesh(Vector3[] targetVerticesArray)
    {
        targetVertices = new Vector3[targetVerticesArray.Length];
        for (int i = 0; i < targetVerticesArray.Length; i++)
            targetVertices[i] = targetVerticesArray[i];
        Destroy(targetMeshGO);
        InstantiateTargetMesh();
        targetLoaded = true;
    }

    /// <summary>
    /// Creating new gameobject from target mesh.
    /// </summary>
    private void InstantiateTargetMesh()
    {
        targetMeshGO = new GameObject();
        targetMeshGO.name = "Target";
        targetMeshGO.AddComponent<MeshFilter>();
        targetMeshGO.AddComponent<MeshRenderer>();
        targetMeshGO.GetComponent<MeshRenderer>().material = targetMaterial;
        Mesh targetMesh = new Mesh();
        targetMesh.SetVertices(targetVertices);
        targetMesh.SetTriangles(triangles, 0);
        targetMesh.RecalculateNormals();
        targetMesh.name = "TargetMesh";
        targetMeshGO.GetComponent<MeshFilter>().mesh = targetMesh;
    }
}
