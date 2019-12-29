using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : MonoBehaviour
{
    [Header("Setup")]
    public bool isRight;
    public bool isFront;
    [Header("Angles")]
    public Vector3 angles;
    public Vector3 anglesScaled;
    public Vector3 localAngles;
    public Vector3 localAnglesClamped;
    public Vector3 locAngClampedScaled;
    [Header("Size")]
    public Vector3 wingSize;
    public Vector3 wingSizeAbs;
    public float wingArea;
    [Header("Force Created")]
    public Vector3 force;
    
    private Rigidbody rb;

    FindCenterOfWing center;

    private void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        wingArea = box.size.x * box.size.z;

        center = GetComponent<FindCenterOfWing>();
    }

    private void Update()
    {
        // Rysowanie wektora normalnego
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.yellow);
        // Obliczanie rzutów skrzydła na płaszczyzny
        wingSize = transform.up * wingArea;
        wingSizeAbs = VectorAbs(wingSize);
        
        // Kąty lokalne
        localAngles = transform.localRotation.eulerAngles;
        localAnglesClamped = ClampVector(transform.localRotation.eulerAngles);
        locAngClampedScaled = localAnglesClamped / 90f;
        // Różnica rotacji skrzydła i ciała
        angles = ClampVector(transform.localRotation.eulerAngles - GetComponentInParent<Transform>().rotation.eulerAngles);
        // Skalowana różnica rotacji
        anglesScaled = angles / 90f;

        // Obliczanie wektora siły
        force = -Vector3.Scale(wingSizeAbs, center.wingCenterMovement);
        Debug.DrawLine(transform.position, transform.position + force, Color.magenta);

        // Stop wing
        rb.angularVelocity = rb.angularVelocity * (1f - Time.deltaTime);
    }

    Vector3 ClampVector(Vector3 v)
    {
        return new Vector3(v.x < 180f ? v.x : v.x - 360f, v.y < 180f ? v.y : v.y - 360f, v.z < 180f ? v.z : v.z - 360f);
    }

    Vector3 VectorAbs(Vector3 v)
    {
        return new Vector3(v.x > 0 ? v.x : -v.x, v.y > 0 ? v.y : -v.y, v.z > 0 ? v.z : -v.z);
    }
}
