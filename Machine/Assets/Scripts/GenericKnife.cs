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
    /// Debugging informations about raycasts and mesh updates.
    /// </summary>
    public bool debugEnabled = false;
    public static int meshUpdateCounter = 0;

    /// <summary>
    /// Start points form which the raycasts are sent.
    /// </summary>
    public static Vector3[] edges;
    /// <summary>
    /// Number of all raycasts.
    /// </summary>
    public static int numberOfPoints;
    /// <summary>
    /// Maximum raycast range. 
    /// </summary>
    public static float rayRange = 1f;
    /// <summary>
    /// Normalized direction of raycasts.
    /// </summary>
    public static Vector3 cutDirectionLocal = new Vector3(-1f, 0f, 0f);

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
    public bool forceKnifeCreation;

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

    /*
    private void Start()
    {
        // Setting range of rays.
        rayRange = rangeOfRaycast;
        // Setting direction of cutting.
        cutDir = cutDirection.normalized;
        // Moving global point from which rays are sent.
        head.position = head.position + (Vector3.right * rayRange);
        // Calculating total number of raycasts.
        numberOfPoints = pointsInFirstLayer + 1 + numOfCircles * pointsPerCircle;
        // Allocating memory for all edges.
        edges = new Vector3[numberOfPoints];
        // Allocating memory for all offset vectors.
        offsets = new Vector3[numberOfPoints];
        // Calculating radius of knife sphere.
        distance = head.transform.lossyScale.x * 0.5f;

        // Setting offsets.
        offsets[0] = transform.rotation * (distance * Vector3.down);
        // First layer. Top layer.
        for (int i = 0; i < pointsInFirstLayer; i++)
        {
            offsets[1 + i] = transform.rotation * (Quaternion.Euler(0f, i * 360f / pointsInFirstLayer, 0f) * (Quaternion.Euler(deflectionAngleOfFirstLayer, 0f, 0f) * (Vector3.down * distance)));
        }
        // Other layers.
        for (int layer = 0; layer < numOfCircles; layer++)
        {
            for (int point = 0; point < pointsPerCircle; point++)
            {
                offsets[1 + pointsInFirstLayer + point + layer * pointsPerCircle] = transform.rotation * (Quaternion.Euler(0f, point * 360f / pointsPerCircle, 0f)
                                                                                  * (Quaternion.Euler(0f, 0f, -90f * layer / numOfCircles) * (distance * Vector3.right)));
            }
        }

        // Updating positions of all edges.
        UpdateEdgesPositions();
    }*/

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

    private void TryToCut()
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        List<GameObject> hittedGO = new List<GameObject>();

        for(int edge_ID = 0; edge_ID < edges.Length; edge_ID++)
        {
            // Projecting raycasts from knife edges.
            if (Physics.Raycast(edges[edge_ID], cutDirectionLocal, out RaycastHit hit, rangeOfRaycast))
            {
                if (!hit.transform.CompareTag("GeneratedCylinder")) break;

                hits.Add(hit);

                GameObject actuallyHitted = hit.collider.gameObject;
                if (!hittedGO.Contains(actuallyHitted)) hittedGO.Add(actuallyHitted);
            }
        }

        for(int hittedGO_ID = 0; hittedGO_ID < hittedGO.Count; hittedGO_ID++)
        {
            GameObject actuallyHittedGO = hittedGO[hittedGO_ID];
            MeshCollider hittedMeshCollider = actuallyHittedGO.GetComponent<MeshCollider>();
            Vector3[] meshVertices = hittedMeshCollider.sharedMesh.vertices;
            int[] meshTriangles = hittedMeshCollider.sharedMesh.triangles;

            bool needToUpdateMesh = false;
            for (int hit_ID = 0; hit_ID < hits.Count; hit_ID++)
            {
                RaycastHit hit = hits[hit_ID];
                if (hit.transform.gameObject == actuallyHittedGO)
                {
                    int triangle_ID = hit.triangleIndex;
                    Vector3 hitPoint = hit.point;
                    float hitPointMagnitudeRelativeToCenter =
                        new Vector3(hitPoint.x, 0f, hitPoint.z).magnitude - (rangeOfRaycast - hit.distance);

                    for (int triangleVertex = 0; triangleVertex < 3; triangleVertex++)
                    {
                        Vector3 actualVertexPoint = meshVertices[meshTriangles[(3 * triangle_ID) + triangleVertex]];
                        float vertexHeight = actualVertexPoint.y;
                        actualVertexPoint.y = 0f;
                        float actualVertexPointMagnitude = actualVertexPoint.magnitude;
                        if (actualVertexPointMagnitude > hitPointMagnitudeRelativeToCenter)
                            actualVertexPointMagnitude = hitPointMagnitudeRelativeToCenter;
                        actualVertexPointMagnitude = Mathf.Clamp(actualVertexPointMagnitude, 0f, float.MaxValue);
                        meshVertices[meshTriangles[(3 * triangle_ID) + triangleVertex]] =
                            (actualVertexPoint.normalized * actualVertexPointMagnitude) +
                            (vertexHeight * Vector3.up);
                    }
                    needToUpdateMesh = true;
                }
            }

            if (needToUpdateMesh)
            {
                Mesh mesh = hittedMeshCollider.sharedMesh;
                mesh.SetVertices(meshVertices);
                hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh = mesh;
                hittedGO[hittedGO_ID].GetComponent<MeshFilter>().mesh.RecalculateNormals();
                hittedMeshCollider.sharedMesh = mesh;

                meshUpdateCounter++;
            }
        }

        if (debugEnabled && hittedGO.Count != 0 && hits.Count != 0)
            Debug.Log("Updated " + meshUpdateCounter + " times, Hitted " + hittedGO.Count + " objects, " + hits.Count + " times.");

        hits.Clear();
        hittedGO.Clear();
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
