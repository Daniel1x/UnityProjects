using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float area = 1f;
    public Vector3 view = Vector3.one.normalized;
    public Vector3 speedV;
    public Vector3 normal;

    public Vector3 globalRzut;
    public Vector3 pow;

    public float area2;
    public float areaB;

    Vector3 pk1;
    Vector3 pk2;
    Vector3 pk3;
    Vector3 pk4;
    Vector3 pk5;
    Vector3 pk6;
    Vector3 pk7;
    Vector3 pk8;

    Vector3 rzut1;
    Vector3 rzut2;
    Vector3 rzut3;
    Vector3 rzut4;    

    private void OnDrawGizmos()
    {
        speedV = GetComponent<Rigidbody>().velocity;

        normal = transform.up;

        globalRzut = area * transform.up;

        pk1 = transform.rotation * new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
        pk2 = transform.rotation * new Vector3(transform.lossyScale.x, transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        pk3 = transform.rotation * new Vector3(-transform.lossyScale.x, transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        pk4 = transform.rotation * new Vector3(-transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
        pk5 = transform.rotation * new Vector3(transform.lossyScale.x, -transform.lossyScale.y, transform.lossyScale.z) * 0.5f;
        pk6 = transform.rotation * new Vector3(transform.lossyScale.x, -transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        pk7 = transform.rotation * new Vector3(-transform.lossyScale.x, -transform.lossyScale.y, -transform.lossyScale.z) * 0.5f;
        pk8 = transform.rotation * new Vector3(-transform.lossyScale.x, -transform.lossyScale.y, transform.lossyScale.z) * 0.5f;

        Gizmos.DrawLine(transform.position, transform.position + pk1);
        Gizmos.DrawLine(transform.position, transform.position + pk2);
        Gizmos.DrawLine(transform.position, transform.position + pk3);
        Gizmos.DrawLine(transform.position, transform.position + pk4);
        Gizmos.DrawLine(transform.position, transform.position + pk5);
        Gizmos.DrawLine(transform.position, transform.position + pk6);
        Gizmos.DrawLine(transform.position, transform.position + pk7);
        Gizmos.DrawLine(transform.position, transform.position + pk8);

        rzut1 = Vector3.ProjectOnPlane(pk1, speedV);
        rzut2 = Vector3.ProjectOnPlane(pk2, speedV);
        rzut3 = Vector3.ProjectOnPlane(pk3, speedV);
        rzut4 = Vector3.ProjectOnPlane(pk4, speedV);

        Gizmos.DrawLine(transform.position + speedV, transform.position + speedV + rzut1);
        Gizmos.DrawLine(transform.position + speedV, transform.position + speedV + rzut2);
        Gizmos.DrawLine(transform.position + speedV, transform.position + speedV + rzut3);
        Gizmos.DrawLine(transform.position + speedV, transform.position + speedV + rzut4);

        area = Vector3.Distance(rzut1, rzut2) * Vector3.Distance(rzut2, rzut3);

        //area2 = Mathf.Abs(Vector3.Dot(rzut3 - rzut2, rzut1 - rzut2) + Mathf.Abs(Vector3.Dot(rzut1 - rzut4, rzut3 - rzut4))) * 0.5f;
        area2 = TriangleArea(rzut1, rzut2, rzut3) + TriangleArea(rzut3, rzut4, rzut1);
        areaB = Brahmagupt(rzut1, rzut2, rzut3, rzut4);
    }

    private float TriangleArea(Vector3 v1,Vector3 v2,Vector3 v3)
    {
        float a = Vector3.Distance(v1, v2);
        float b = Vector3.Distance(v2, v3);
        float c = Vector3.Distance(v3, v1);
        float p = 0.5f * (a + b + c);
        float area = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
        return area;
    }

    private float Brahmagupt(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
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