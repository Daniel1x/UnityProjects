using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedTransform : MonoBehaviour
{
    BoxCollider boxCollider;
    Vector3 pivotToCenter;

    [Header("Actual Transform Setup")]
    public Vector3 center;
    public Vector3 centerGlobal;
    public Quaternion rotation;
    public Vector3 normalVector;
    public Vector3 normalUpVector;

    //[Header("Last Transform Setup")]
    private Vector3 lastCenter;
    private Vector3 lastCenterGlobal;
    private Quaternion lastRotation;
    private Vector3 lastNormalVector;

    [Header("Differences between frames")]
    private Vector3 centerMovement;
    private Vector3 centerMovementGlobal;
    private Quaternion rotationChange;
    private Vector3 normalVectorDifference;
    private Vector3 areaProjectionDifference;

    [Header("Area")]
    public float area;
    public Vector3 areaProjection;
    public Vector3 areaProjectionUp;
    public Vector3 areaProjectionAbs;
    private Vector3 lastAreaProjection;
    private Vector3 lastAreaProjectionAbs;

    [Header("Vertices")]
    public Vector3[] vertices = new Vector3[8];
    public Vector3[] verticesGlobal = new Vector3[8];

    [Header("Visualisation")]
    public bool debugEnabled = true;
    private bool lastStatus = true;

    [Header("Collision Event")]
    public bool crashed = false;
    public string colliderTag = null;

    private ButterflyAgentToTarget agent;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        pivotToCenter = Vector3.Scale(boxCollider.center, transform.lossyScale);
        area = boxCollider.size.x * boxCollider.size.z * transform.localScale.x * transform.localScale.z;
        lastStatus = debugEnabled;

        SaveActualTransform();
        SaveLastTransform();
        CalculateDifferences();

        agent = GetComponentInParent<ButterflyAgentToTarget>();
    }

    private void Update()
    {
        area = boxCollider.size.x * boxCollider.size.z * transform.localScale.x * transform.localScale.z;
    }

    private void FixedUpdate()
    {
        SaveLastTransform();
        SaveActualTransform();
        CalculateDifferences();
        ShowVectors();
    }

    private void SaveLastTransform()
    {
        lastCenter = center;
        lastCenterGlobal = centerGlobal;
        lastRotation = rotation;
        lastNormalVector = normalVector;
        lastAreaProjection = areaProjection;
        lastAreaProjectionAbs = areaProjectionAbs;
    }

    private void SaveActualTransform()
    {
        center = transform.rotation * pivotToCenter;
        centerGlobal = center + transform.position;
        rotation = transform.rotation;
        normalVector = transform.up;
        normalUpVector = (Vector3.Angle(transform.up, Vector3.up) <= 90f) ? transform.up : -transform.up;
        areaProjection = area * normalVector;
        areaProjectionUp = area * normalUpVector;
        areaProjectionAbs = new Vector3(Mathf.Abs(areaProjection.x), Mathf.Abs(areaProjection.y), Mathf.Abs(areaProjection.z));

        SaveVertices();
    }

    private void SaveVertices()
    {
        vertices[0] = rotation * new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
        vertices[1] = rotation * new Vector3(transform.lossyScale.x, transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        vertices[2] = rotation * new Vector3(-transform.lossyScale.x, transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        vertices[3] = rotation * new Vector3(-transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
        vertices[4] = rotation * new Vector3(transform.lossyScale.x, -transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
        vertices[5] = rotation * new Vector3(transform.lossyScale.x, -transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        vertices[6] = rotation * new Vector3(-transform.lossyScale.x, -transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        vertices[7] = rotation * new Vector3(-transform.lossyScale.x, -transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
        for (int i = 0; i < 8; i++)
        {
            verticesGlobal[i] = vertices[i] + transform.position;
        }
    }

    private void CalculateDifferences()
    {
        centerMovement = center - lastCenter;
        centerMovementGlobal = centerGlobal - centerMovementGlobal;
        rotationChange = Quaternion.Euler(rotation.eulerAngles - rotation.eulerAngles);
        normalVectorDifference = normalVector - lastNormalVector;
        areaProjectionDifference = areaProjectionAbs - lastAreaProjectionAbs;
    }

    private void ShowVectors()
    {
        Triggered();
        if (!debugEnabled) return;

        Debug.DrawLine(centerGlobal, centerGlobal + normalVector, Color.blue);
        Debug.DrawLine(centerGlobal, centerGlobal + lastNormalVector, Color.green);
        Debug.DrawLine(centerGlobal, centerGlobal + normalUpVector, Color.red);
        Debug.DrawLine(centerGlobal, lastCenterGlobal, Color.yellow);
        Debug.DrawLine(centerGlobal + normalVector, centerGlobal + lastNormalVector, Color.white);
    }

    private void Triggered()
    {
        if (lastStatus != debugEnabled)
        {
            Debug.Log("Normal Vector Color: Blue");
            Debug.Log("Last Normal Vector Color: Green");
            Debug.Log("Normal Up Vector Color: Red");
            Debug.Log("Difference of Normals Color: White");
            Debug.Log("Center Movement Vector Color: Yellow");
        }
        lastStatus = debugEnabled;
    }

    public float ProjectOnPlane(Vector3 normal, bool triangleInsteadOfQuadrangle = true)
    {
        Vector3[] projectedVertex = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            projectedVertex[i] = Vector3.ProjectOnPlane(vertices[i], normal.normalized);
        }
        float area = 0f;
        if (triangleInsteadOfQuadrangle)
        {
            area = TriangleArea(projectedVertex[0], projectedVertex[1], projectedVertex[2])
                 + TriangleArea(projectedVertex[2], projectedVertex[3], projectedVertex[1]);
        }
        else
        {
            area = QuadrangleArea(projectedVertex[0], projectedVertex[1], projectedVertex[2], projectedVertex[3]);
        }
        return area;
    }

    private void OnCollisionEnter(Collision collision)
    {
        crashed = true;
        colliderTag = collision.transform.tag;
        if(agent!=null)
            agent.AddCollisionReward(collision.transform.tag);
    }

    private void OnCollisionStay(Collision collision)
    {
        crashed = true;
        colliderTag = collision.transform.tag;
        if(agent!=null)
            agent.AddCollisionReward(collision.transform.tag,-0.00001f);
    }

    private void OnCollisionExit(Collision collision)
    {
        crashed = false;
        colliderTag = null;
    }

    private float TriangleArea(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float a = Vector3.Distance(v1, v2);
        float b = Vector3.Distance(v2, v3);
        float c = Vector3.Distance(v3, v1);
        float p = 0.5f * (a + b + c);
        float area = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
        return area;
    }

    private float QuadrangleArea(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        float a = Vector3.Distance(v1, v2);
        float b = Vector3.Distance(v2, v3);
        float c = Vector3.Distance(v3, v4);
        float d = Vector3.Distance(v4, v1);
        float p = 0.5f * (a + b + c + d);
        float fi = Mathf.Deg2Rad * 0.5f * (Vector3.Angle(v1 - v2, v3 - v2) + Vector3.Angle(v1 - v4, v3 - v4));
        float area = Mathf.Sqrt((p - a) * (p - b) * (p - c) * (p - d) - (a * b * c * d * Mathf.Pow(Mathf.Cos(fi), 2f)));
        return area;
    }
}
