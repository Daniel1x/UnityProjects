using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericKnife : MonoBehaviour
{
    /// <summary>
    /// Number of raycasts on blade.
    /// </summary>
    [Range(0, 180)] public int numberOfPoints = 3;
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
    /// Defines type of cuts handler.
    /// </summary>
    public bool handleCutsByLayer = true;
    /// <summary>
    /// Defines how much the thickness of the cylinder can be reduced.
    /// </summary>
    [Range(0.0001f, 0.1f)] public float minimumMetalThickness = 0.01f;

    /// <summary>
    /// Start points form which the raycasts are sent.
    /// </summary>
    private Vector3[] edges;
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
    
    public void CreateKnife(int numberOfPoints, float rangeOfRaycast)
    {
        this.numberOfPoints = numberOfPoints;
        this.rangeOfRaycast = rangeOfRaycast;
        
        cutDirectionLocal = transform.rotation * cutDirectionGlobal.normalized;
        headPositionOffset = -cutDirectionLocal * rangeOfRaycast;
        edges = new Vector3[numberOfPoints];
        offsets = new Vector3[numberOfPoints];
        radiusOfSphere = transform.lossyScale.x * 0.5f;

        Vector3 closestPointOnTheSphereInCutDirection = cutDirectionLocal * radiusOfSphere;

        for (int i = 0; i < numberOfPoints; i++)
            offsets[i] = Quaternion.Euler(0f, 0f, -90f + (180f * i / (numberOfPoints - 1))) * closestPointOnTheSphereInCutDirection;

        UpdateEdgesPositions();

        knifeCreated = true;
    }

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;

        if (!knifeCreated)
        {
            knifeCreated = true;
            CreateKnife(numberOfPoints, rangeOfRaycast);
        }
        if (!knifeCreated) return;
        
        UpdateEdgesPositions();
        TryToCut();

        Debug.Log(1000f * (Time.realtimeSinceStartup - startTime) + " ms.");
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
                        actualVertexPointMagnitude = Mathf.Clamp(actualVertexPointMagnitude, minimumMetalThickness, float.MaxValue);
                        meshVertices[actualVertexIndex] = (actualVertexPoint.normalized * actualVertexPointMagnitude)
                                                         + (vertexHeight * Vector3.up);
                    }
                    needToUpdateMesh = true;
                }
            }

            if (needToUpdateMesh)
            {
                Mesh mesh = hittedMeshCollider.sharedMesh;
                mesh.MarkDynamic();
                mesh.SetVertices(meshVertices);
                cuts.hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh = mesh;
                cuts.hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh.RecalculateNormals();
                hittedMeshCollider.sharedMesh = mesh;
            }
        }
    }
    
    private void HandleCutsByLayer(Cuts cuts)
    {
        for (int hittedGO_ID = 0; hittedGO_ID < cuts.hittedGO.Count; hittedGO_ID++)
        {
            GameObject actuallyHittedGO = cuts.hittedGO[hittedGO_ID];
            MeshCollider hittedMeshCollider = actuallyHittedGO.GetComponent<MeshCollider>();
            Mesh hittedMesh = hittedMeshCollider.sharedMesh;
            hittedMesh.MarkDynamic();
            Vector3[] meshVertices = hittedMesh.vertices;
            int meshVerticesLength = meshVertices.Length;
            int[] meshTriangles = hittedMesh.triangles;
            bool needToUpdateMesh = false;

            for (int hit_ID = 0; hit_ID < cuts.hits.Count; hit_ID++)
            {
                RaycastHit hit = cuts.hits[hit_ID];
                if (hit.transform.gameObject == actuallyHittedGO)
                {
                    Info meshInfo = actuallyHittedGO.GetComponent<GenericMeshInfo>().info;
                    int numberOfVerticesPerLayer = meshInfo.numberOfVerticesPerLayer;
                    int numberOfLayers = meshInfo.numberOfLayers;
                    int triangle_ID = hit.triangleIndex;
                    int[] layers_ID = GetLayersID(triangle_ID, meshTriangles, numberOfVerticesPerLayer);

                    Vector3 hitPoint = hit.point;
                    float hitPointMagnitudeRelativeToCenter =
                        new Vector3(hitPoint.x, 0f, hitPoint.z).magnitude - (rangeOfRaycast - hit.distance);

                    for (int vertexLayer_ID = 0; vertexLayer_ID < layers_ID.Length; vertexLayer_ID++)
                    {
                        int layer_ID = layers_ID[vertexLayer_ID];
                        if (layer_ID >= meshInfo.numberOfLayers) continue;

                        int firstVertexInLayer_ID = layer_ID * numberOfVerticesPerLayer;
                        Vector3 firstVertexInLayer = meshVertices[firstVertexInLayer_ID];
                        float actualLayerHeight = firstVertexInLayer.y;
                        firstVertexInLayer.y = 0f;
                        float actualLayerMagnitude = firstVertexInLayer.magnitude;
                        if (actualLayerMagnitude > hitPointMagnitudeRelativeToCenter)
                            actualLayerMagnitude = hitPointMagnitudeRelativeToCenter;
                        actualLayerMagnitude = Mathf.Clamp(actualLayerMagnitude, minimumMetalThickness, float.MaxValue);
                        meshInfo.magnitudesOfLayers[layer_ID] = actualLayerMagnitude;

                        for(int vertex_ID = 0; vertex_ID < numberOfVerticesPerLayer; vertex_ID++)
                        {
                            int actualVertexIndex = firstVertexInLayer_ID + vertex_ID;
                            if (actualVertexIndex >= meshVerticesLength) break;

                            Vector3 actualVertex = meshVertices[actualVertexIndex];
                            actualVertex.y = 0f;
                            actualVertex.Normalize();

                            meshVertices[actualVertexIndex] = actualVertex * actualLayerMagnitude + actualLayerHeight * Vector3.up;
                        }
                    }
                    needToUpdateMesh = true;
                }
            }

            if (needToUpdateMesh)
            {
                hittedMesh.MarkDynamic();
                hittedMesh.SetVertices(meshVertices);
                MeshFilter meshFilter = cuts.hittedGO[hittedGO_ID].GetComponent<MeshFilter>();
                meshFilter.mesh = hittedMesh;
                meshFilter.mesh.RecalculateNormals();
                hittedMeshCollider.sharedMesh = hittedMesh;
            }
        }
    }

    public static int[] GetLayersID(int triangle_ID, int[] meshTriangles, int numberOfVerticesPerLayer)
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
        if (edges == null) CreateKnife(numberOfPoints, rangeOfRaycast);
        for (int i = 0; i < edges.Length; i++)
        {
            Gizmos.DrawSphere(edges[i], 0.02f);
        }
        for (int i = 0; i < edges.Length; i++)
        {
            Gizmos.DrawSphere(edges[i] + (transform.rotation * cutDirectionLocal * rangeOfRaycast), 0.01f);
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