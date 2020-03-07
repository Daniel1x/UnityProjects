using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKnife : MonoBehaviour
{
    [SerializeField] [Range(1f, 100f)] private float raycastRange = 5f;
    [SerializeField] private Vector3 cuttingDirection = Vector3.left;
    [SerializeField] [Range(0f, 1f)] private float sizeIncreaseFactor = 0.1f;

    private Transform thisTransform;

    private void Start()
    {
        thisTransform = transform;
    }

    private void Update()
    {
        Debug.DrawLine(thisTransform.position, thisTransform.position + raycastRange * cuttingDirection, Color.red);
        if (!Input.anyKey) return;

        if (Physics.Raycast(thisTransform.position,cuttingDirection,out RaycastHit hit, raycastRange))
        {
            if (!hit.transform.CompareTag("GeneratedCylinder")) return;

            GameObject hittedGO = hit.transform.gameObject;
            MeshFilter meshFilter = hittedGO.GetComponent<MeshFilter>();
            meshFilter.sharedMesh.MarkDynamic();
            MeshCollider meshCollider = hittedGO.GetComponent<MeshCollider>();
            meshCollider.sharedMesh.MarkDynamic();
            GenericMeshInfo genericMeshInfo = hittedGO.GetComponent<GenericMeshInfo>();
            Mesh mesh = meshCollider.sharedMesh;
            mesh.MarkDynamic();
            Vector3[] meshVertices = mesh.vertices;
            int meshVerticesLength = meshVertices.Length;
            int[] meshTriangles = mesh.triangles;
            int hittedTriangleIndex = hit.triangleIndex;

            float input = Input.GetAxis("Horizontal") * Time.deltaTime * sizeIncreaseFactor;

            int verticesPerLayer = genericMeshInfo.numberOfVerticesPerLayer;
            int[] triangleVerticesIndex = new int[3];
            triangleVerticesIndex[0] = meshTriangles[3 * hittedTriangleIndex] / verticesPerLayer;
            triangleVerticesIndex[1] = meshTriangles[(3 * hittedTriangleIndex) + 1] / verticesPerLayer;
            triangleVerticesIndex[2] = meshTriangles[(3 * hittedTriangleIndex) + 2] / verticesPerLayer;

            int verticesInLayer = genericMeshInfo.numberOfVerticesPerLayer;
            Debug.Log("N vertives: " + verticesInLayer);
            for(int i = 0; i < verticesInLayer; i++)
            {
                int thisVertexIndex = triangleVerticesIndex[2] * verticesPerLayer + i;

                Vector3 vertex = meshVertices[thisVertexIndex];

                float vertexHeight = vertex.y;
                vertex.y = 0f;
                float vertexMagnitude = Mathf.Clamp(vertex.magnitude + input, 0.00001f, float.MaxValue);
                Vector3 newVertex = vertex.normalized * vertexMagnitude + vertexHeight * Vector3.up;

                meshVertices[thisVertexIndex] = newVertex;
            }

            mesh.SetVertices(meshVertices);
            mesh.RecalculateNormals();
            meshCollider.sharedMesh = mesh;
            meshFilter.sharedMesh = mesh;
        }
    }
}
