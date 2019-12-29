using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftForce : MonoBehaviour
{
    ExtendedTransform extTransform;
    Rigidbody rb;
    
    [Header("Parameters")]
    public float area;
    //public Vector3 areaUp;
    //public float dragCoefficient = 0.5f;
    public float liftCoefficient = 0.5f;
    public float surfacePerpendicularToVelocityVector;
    public float dynamicPressure;
    public float airDensity = 1.2f;
    public Vector3 normalUpVector;
    public Vector3 velocityVector;
    [Header("Forces created by lift")]
    public Vector3 liftForce;
    public bool addForce = false;
    public bool addForceAtPosition = false;

    private void Start()
    {
        extTransform = GetComponent<ExtendedTransform>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        velocityVector = rb.velocity;
        normalUpVector = extTransform.normalUpVector.normalized;
        dynamicPressure = 0.5f * airDensity * velocityVector.magnitude * velocityVector.magnitude;
        liftCoefficient = 0.5f; // 2 * Mathf.PI * angleOfAttack * Mathf.Deg2Rad;
        surfacePerpendicularToVelocityVector = Mathf.Clamp(extTransform.ProjectOnPlane(velocityVector, false), 0.00001f, extTransform.area);
        area = extTransform.area;
        //areaUp = extTransform.areaProjectionUp;
        liftForce = rb.mass * Time.fixedDeltaTime * liftCoefficient * dynamicPressure * area * normalUpVector * CheckDirection();
        liftForce = Vector3.ClampMagnitude(liftForce, 10f);

        AddForce();
        ShowVectors();
    }

    private void ShowVectors()
    {
        if (!extTransform.debugEnabled) return;
        Debug.DrawLine(transform.position, transform.position + liftForce, Color.red);
        Debug.DrawLine(transform.position, transform.position + velocityVector, Color.black);
    }

    private void AddForce()
    {
        if (!addForce || liftForce == null) return;
        if (!addForceAtPosition)
        {
            rb.AddForce(liftForce);
        }
        else
        {
            rb.AddForceAtPosition(liftForce, extTransform.centerGlobal);
        }
    }

    private float CheckDirection()
    {
        return Vector3.Angle(extTransform.normalUpVector, velocityVector) >= 90f ? 1f : -1;
    }
    // LiftCoeff = 2*pi*angle(in radians)
    // Lift = LiftCoeff * density * velocity^2 * area(V) * 0.5;
}
