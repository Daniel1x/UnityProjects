using UnityEngine;

public class Knife : MonoBehaviour
{
    /// <summary>
    /// Points in first layer. These points will be deflected by the given angle.
    /// </summary>
    public int pointsInFirstLayer = 4;
    /// <summary>
    /// The angle by which the points of the first layer of the blade will be deflected.
    /// </summary>
    [Range(0f, 30f)] public float deflectionAngle = 1f;
    /// <summary>
    /// Points per layer of raycasts on circle.
    /// </summary>
    [Range(0, 16)] public int pointsPerCircle = 8;
    /// <summary>
    /// Number of layers of raycasts on blade.
    /// </summary>
    [Range(0, 16)] public int numOfCircles = 2;
    /// <summary>
    /// Direction of projected raycasts.
    /// </summary>
    public Vector3 cutDirection = new Vector3(-1f, 0f, 0f);
    /// <summary>
    /// Position from which the raycasts are sent.
    /// </summary>
    public Transform head;
    /// <summary>
    /// Draw raycasts points. Drawing sphere at the beginning and end of the ray.
    /// </summary>
    public bool drawPoints = true;
    /// <summary>
    /// Range of projected raycasts.
    /// </summary>
    public float rangeOfRaycast = 1f;

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
    public static Vector3 cutDir = new Vector3(-1f, 0f, 0f);

    /// <summary>
    /// Table of offset vectors, used to calculate global position of edges.
    /// </summary>
    private Vector3[] offsets;
    /// <summary>
    /// Radius of knife sphere.
    /// </summary>
    private float distance;
    
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
        for(int i = 0; i < pointsInFirstLayer; i++)
        {
            offsets[1 + i] = transform.rotation * (Quaternion.Euler(0f, i * 360f / pointsInFirstLayer, 0f) * (Quaternion.Euler(deflectionAngle, 0f, 0f) * (Vector3.down * distance)));
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
    }

    private void Update()
    {
        // Updating positions of all edges.
        UpdateEdgesPositions();
    }

    /// <summary>
    /// Drawing spheres on gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawPoints) return;
        for (int i = 0; i < numberOfPoints; i++)
        {
            Gizmos.DrawSphere(edges[i], 0.01f);
        }
        for (int i = 0; i < numberOfPoints; i++)
        {
            Gizmos.DrawSphere(edges[i] + cutDir * rayRange, 0.01f);
        }
    }

    /// <summary>
    /// Update positions of all edges, relative to head position.
    /// </summary>
    private void UpdateEdgesPositions()
    {
        Vector3 pos = head.transform.position;
        for (int i = 0; i < numberOfPoints; i++) edges[i] = pos + offsets[i];
    }
}
