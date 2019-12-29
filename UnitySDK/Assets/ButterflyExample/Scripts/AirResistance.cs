using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirResistance : MonoBehaviour
{
    ExtendedTransform extTransform;
    Rigidbody rb;

    [Header("Parameters")]
    public float dragCoefficient = 0.5f;
    public float surfacePerpendicularToVelocityVector;
    public float dynamicPressure;
    public float maxDynamicPressure = 25f;
    public float airDensity = 1.2f;
    public Vector3 velocityVector;
    [Header("Forces created by air resistance")]
    public Vector3 airResistance;
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
        dynamicPressure = 0.5f * airDensity * velocityVector.magnitude * velocityVector.magnitude;
        dynamicPressure = Mathf.Clamp(dynamicPressure, 0f, maxDynamicPressure);
        surfacePerpendicularToVelocityVector = Mathf.Clamp(extTransform.ProjectOnPlane(velocityVector, false), 0.00001f, extTransform.area);
        airResistance = -velocityVector.normalized * dragCoefficient * surfacePerpendicularToVelocityVector * dynamicPressure * Time.fixedDeltaTime * rb.mass;
        airResistance = Vector3.ClampMagnitude(airResistance, 100f);
        ShowVectors();
        AddForce();
    }

    private void AddForce()
    {
        if (!addForce || airResistance == null || float.IsNaN(airResistance.x)) return;
        if (!addForceAtPosition)
        {
            rb.AddForce(airResistance);
        }
        else
        {
            rb.AddForceAtPosition(airResistance, extTransform.centerGlobal);
        }
    }

    private void ShowVectors()
    {
        if (!extTransform.debugEnabled) return;
        Debug.DrawLine(extTransform.centerGlobal, extTransform.centerGlobal + airResistance, Color.black);
    }

    //Siła oporu:
    //Kierunek = -V.speed
    //Cd - drag coefficient - 0.5 = const
    //Sd - powierzchnia prostopadła do V.speed
    //pd - ciśnienie dynamiczne = (gęstość * |V.speed|^2)/2
    //Wartość wektora: D = Cd * Sd * pd
}
