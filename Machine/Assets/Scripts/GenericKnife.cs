using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericKnife : MonoBehaviour
{
    /// <summary>
    /// Points in first layer. These points will be deflected by the given angle.
    /// </summary>
    [Range(0, 16)] public int pointsInFirstLayer = 4;
    /// <summary>
    /// The angle by which the points of the first layer of the blade will be deflected.
    /// </summary>
    [Range(0f, 30f)] public float deflectionAngleOfFirstLayer = 1f;
    /// <summary>
    /// Points per layer of raycasts on circle.
    /// </summary>
    [Range(0, 16)] public int pointsPerCircle = 8;
    /// <summary>
    /// Number of layers of raycasts on blade.
    /// </summary>
    [Range(0, 16)] public int numOfCircles = 3;
    /// <summary>
    /// Direction of projected raycasts.
    /// </summary>
    private Vector3 cutDirectionGlobal = new Vector3(-1f, 0f, 0f);
    /// <summary>
    /// Position from which the raycasts are sent.
    /// </summary>
    private Vector3 headPositionOffset;
    /// <summary>
    /// Draw raycasts points. Drawing sphere at the beginning and end of the ray.
    /// </summary>
    public bool drawPoints = true;
    /// <summary>
    /// Range of projected raycasts.
    /// </summary>
    public float rangeOfRaycast = 1f;
    /// <summary>
    /// Mesh updates counter.
    /// </summary>
    public static int meshUpdateCounter = 0;
    /// <summary>
    /// Force update of values.
    /// </summary>
    public bool forceKnifeCreation;
    /// <summary>
    /// Defines type of cuts handler.
    /// </summary>
    public bool handleCutsByLayer;

    /// <summary>
    /// Start points form which the raycasts are sent.
    /// </summary>
    private Vector3[] edges;
    /// <summary>
    /// Number of all raycasts.
    /// </summary>
    private int numberOfPoints;
    /// <summary>
    /// Maximum raycast range. 
    /// </summary>
    private float rayRange = 1f;
    /// <summary>
    /// Normalized direction of raycasts.
    /// </summary>
    private Vector3 cutDirectionLocal = new Vector3(-1f, 0f, 0f);
    /// <summary>
    /// Table of offset vectors, used to calculate global position of edges.
    /// </summary>
    private Vector3[] offsets;
    /// <summary>
    /// Radius of knife sphere.
    /// </summary>
    private float radiusOfSphere;
    /// <summary>
    /// Is knife created. Allows to update knife position.
    /// </summary>
    private bool knifeCreated = false;

    public void CreateKnife(int pointsInFirstLayer, float deflectionAngleOfFirstLayer, int pointsPerCircle, int numOfCircles, Vector3 cutDirectionGlobal, float rangeOfRaycast)
    {
        this.pointsInFirstLayer = pointsInFirstLayer;
        this.deflectionAngleOfFirstLayer = deflectionAngleOfFirstLayer;
        this.pointsPerCircle = pointsPerCircle;
        this.numOfCircles = numOfCircles;
        this.cutDirectionGlobal = cutDirectionGlobal.normalized;
        this.rangeOfRaycast = rangeOfRaycast;

        // Setting range of rays.
        rayRange = rangeOfRaycast;
        // Setting direction of cutting.
        cutDirectionLocal = transform.rotation * cutDirectionGlobal.normalized;
        // Moving global point from which rays are sent.
        headPositionOffset = -cutDirectionLocal * rangeOfRaycast;
        // Calculating total number of raycasts.
        numberOfPoints = 1 + pointsInFirstLayer + numOfCircles * pointsPerCircle;
        // Allocating memory for all edges.
        edges = new Vector3[numberOfPoints];
        // Allocating memory for all offset vectors.
        offsets = new Vector3[numberOfPoints];
        // Calculating radius of knife sphere.
        radiusOfSphere = transform.lossyScale.x * 0.5f;

        Vector3 closestPointOnTheSphereInCutDirection = cutDirectionLocal * radiusOfSphere;
        // Setting offsets.
        offsets[0] = closestPointOnTheSphereInCutDirection;
        // First layer.
        for(int i = 0; i < pointsInFirstLayer; i++)
        {
            offsets[1 + i] = Quaternion.Euler(i * 360f / pointsInFirstLayer, 0f, 0f) * Quaternion.Euler(0f, deflectionAngleOfFirstLayer, 0f) * closestPointOnTheSphereInCutDirection;
        }
        // Other layers.
        for (int layer = 0; layer < numOfCircles; layer++)
        {
            for (int point = 0; point < pointsPerCircle; point++)
            {
                offsets[1 + pointsInFirstLayer + point + layer * pointsPerCircle] =
                    Quaternion.Euler(point * 360f / pointsPerCircle, 0f, 0f) *
                    Quaternion.Euler(0f, (layer + 1) * 90f / numOfCircles, 0f) *
                    closestPointOnTheSphereInCutDirection;
            }
        }

        // Updating positions of all edges.
        UpdateEdgesPositions();

        knifeCreated = true;
    }

    private void Update()
    {
        if (forceKnifeCreation && !knifeCreated)
        {
            knifeCreated = true;
            CreateKnife(pointsInFirstLayer, deflectionAngleOfFirstLayer, pointsPerCircle, numOfCircles, cutDirectionGlobal, rangeOfRaycast);
        }

        if (!knifeCreated) return;

        // Updating positions of all edges.
        UpdateEdgesPositions();
        TryToCut();
    }

    private class Cuts
    {
        public List<RaycastHit> hits = new List<RaycastHit>();
        public List<GameObject> hittedGO = new List<GameObject>();
    }

    private Cuts GetCuts()
    {
        Cuts cuts = new Cuts();

        for (int edge_ID = 0; edge_ID < edges.Length; edge_ID++)
        {
            if (Physics.Raycast(edges[edge_ID], cutDirectionLocal, out RaycastHit hit, rangeOfRaycast))
            {
                if (!hit.transform.CompareTag("GeneratedCylinder")) break;

                cuts.hits.Add(hit);

                GameObject actuallyHitted = hit.collider.gameObject;
                if (!cuts.hittedGO.Contains(actuallyHitted)) cuts.hittedGO.Add(actuallyHitted);
            }
        }

        return cuts;
    }

    private void HandleCuts(Cuts cuts)
    {
        for (int hittedGO_ID = 0; hittedGO_ID < cuts.hittedGO.Count; hittedGO_ID++)
        {
            GameObject actuallyHittedGO = cuts.hittedGO[hittedGO_ID];
            MeshCollider hittedMeshCollider = actuallyHittedGO.GetComponent<MeshCollider>();
            Vector3[] meshVertices = hittedMeshCollider.sharedMesh.vertices;
            int[] meshTriangles = hittedMeshCollider.sharedMesh.triangles;
            bool needToUpdateMesh = false;

            for (int hit_ID = 0; hit_ID < cuts.hits.Count; hit_ID++)
            {
                RaycastHit hit = cuts.hits[hit_ID];
                if (hit.transform.gameObject == actuallyHittedGO)
                {
                    int triangle_ID = hit.triangleIndex;
                    Vector3 hitPoint = hit.point;
                    float hitPointMagnitudeRelativeToCenter =
                        new Vector3(hitPoint.x, 0f, hitPoint.z).magnitude - (rangeOfRaycast - hit.distance);

                    for (int triangleVertex = 0; triangleVertex < 3; triangleVertex++)
                    {
                        int actualVertexIndex = meshTriangles[(3 * triangle_ID) + triangleVertex];
                        Vector3 actualVertexPoint = meshVertices[actualVertexIndex];
                        float vertexHeight = actualVertexPoint.y;
                        actualVertexPoint.y = 0f;
                        float actualVertexPointMagnitude = actualVertexPoint.magnitude;
                        if (actualVertexPointMagnitude > hitPointMagnitudeRelativeToCenter)
                            actualVertexPointMagnitude = hitPointMagnitudeRelativeToCenter;
                        actualVertexPointMagnitude = Mathf.Clamp(actualVertexPointMagnitude, 0f, float.MaxValue);
                        meshVertices[actualVertexIndex] = (actualVertexPoint.normalized * actualVertexPointMagnitude)
                                                         + (vertexHeight * Vector3.up);
                    }
                    needToUpdateMesh = true;
                }
            }

            if (needToUpdateMesh)
            {
                Mesh mesh = hittedMeshCollider.sharedMesh;
                mesh.SetVertices(meshVertices);
                cuts.hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh = mesh;
                cuts.hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh.RecalculateNormals();
                hittedMeshCollider.sharedMesh = mesh;

                meshUpdateCounter++;
            }
        }
    }

    private void HandleCutsByLayer(Cuts cuts)
    {
        for (int hittedGO_ID = 0; hittedGO_ID < cuts.hittedGO.Count; hittedGO_ID++)
        {
            GameObject actuallyHittedGO = cuts.hittedGO[hittedGO_ID];
            MeshCollider hittedMeshCollider = actuallyHittedGO.GetComponent<MeshCollider>();
            Vector3[] meshVertices = hittedMeshCollider.sharedMesh.vertices;
            int meshVerticesLength = meshVertices.Length;
            int[] meshTriangles = hittedMeshCollider.sharedMesh.triangles;
            bool needToUpdateMesh = false;
            
            for (int hit_ID = 0; hit_ID < cuts.hits.Count; hit_ID++)
            {
                RaycastHit hit = cuts.hits[hit_ID];
                if (hit.transform.gameObject == actuallyHittedGO)
                {
                    GenericMeshInfo meshInfo = actuallyHittedGO.GetComponent<GenericMeshInfo>();
                    int numberOfVerticesPerLayer = meshInfo.NumberOfVerticesPerLayer;
                    int numberOfLayers = meshInfo.NumberOfLayers;
                    int triangle_ID = hit.triangleIndex;
                    int[] layers_ID = GetLayersID(triangle_ID, meshTriangles, numberOfVerticesPerLayer);

                    Vector3 hitPoint = hit.point;
                    float hitPointMagnitudeRelativeToCenter =
                        new Vector3(hitPoint.x, 0f, hitPoint.z).magnitude - (rangeOfRaycast - hit.distance);

                    for(int vertexLayer_ID = 0; vertexLayer_ID < layers_ID.Length; vertexLayer_ID++)
                    {
                        int layer_ID = layers_ID[vertexLayer_ID];
                        for (int vertex_ID = 0; vertex_ID < 3 * numberOfVerticesPerLayer; vertex_ID++)
                        {
                            int actualVertexIndex = layers_ID[vertexLayer_ID] * numberOfVerticesPerLayer + vertex_ID;
                            if (actualVertexIndex >= meshVerticesLength) break;
                            Vector3 actualVertexPoint = meshVertices[actualVertexIndex];
                            float actualVertexHeight = actualVertexPoint.y;
                            actualVertexPoint.y = 0f;
                            float actualVertexPointMagnitude = actualVertexPoint.magnitude;
                            if (actualVertexPointMagnitude > hitPointMagnitudeRelativeToCenter)
                                actualVertexPointMagnitude = hitPointMagnitudeRelativeToCenter;
                            actualVertexPointMagnitude = Mathf.Clamp(actualVertexPointMagnitude, 0f, float.MaxValue);
                            meshVertices[actualVertexIndex] = actualVertexPoint.normalized * actualVertexPointMagnitude
                                + actualVertexHeight * Vector3.up;
                        }
                    }
                    needToUpdateMesh = true;
                }
            }

            if (needToUpdateMesh)
            {
                Mesh mesh = hittedMeshCollider.sharedMesh;
                mesh.SetVertices(meshVertices);
                cuts.hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh = mesh;
                cuts.hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh.RecalculateNormals();
                hittedMeshCollider.sharedMesh = mesh;

                meshUpdateCounter++;
            }
        }
    }

    private int[] GetLayersID(int triangle_ID, int[] meshTriangles, int numberOfVerticesPerLayer)
    {
        int[] layers_ID = new int[2];
        int[] triangleVertices = new int[3];

        triangleVertices[0] = meshTriangles[3 * triangle_ID];
        triangleVertices[1] = meshTriangles[(3 * triangle_ID) + 1];
        triangleVertices[2] = meshTriangles[(3 * triangle_ID) + 2];

        layers_ID[0] = triangleVertices[0] / numberOfVerticesPerLayer;
        layers_ID[1] = triangleVertices[1] / numberOfVerticesPerLayer;
        if (layers_ID[0] == layers_ID[1])
            layers_ID[1] = triangleVertices[2] / numberOfVerticesPerLayer;

        return layers_ID;
    }
    
    private void TryToCut()
    {
        if (handleCutsByLayer)
        {
            HandleCutsByLayer(GetCuts());
        }
        else
        {
            HandleCuts(GetCuts());
        }
    }

    /// <summary>
    /// Drawing spheres on gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawPoints) return;
        for (int i = 0; i < numberOfPoints; i++)
        {
            Gizmos.DrawSphere(edges[i], 0.02f);
        }
        for (int i = 0; i < numberOfPoints; i++)
        {
            Gizmos.DrawSphere(edges[i] + (transform.rotation * cutDirectionLocal * rayRange), 0.01f);
        }
    }

    /// <summary>
    /// Update positions of all edges, relative to head position.
    /// </summary>
    private void UpdateEdgesPositions()
    {
        Vector3 actualPositionOfHead = transform.position + headPositionOffset;
        for (int i = 0; i < numberOfPoints; i++) edges[i] = actualPositionOfHead + offsets[i];
    }
}
