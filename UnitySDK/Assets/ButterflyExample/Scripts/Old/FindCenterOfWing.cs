using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCenterOfWing : MonoBehaviour
{
    BoxCollider thisCollider;
    Vector3 distanceV;

    [Header("Actual Position Setup")]
    public Vector3 nWingCenter;
    public Vector3 nWingCenterGlobal;
    public Vector3 normalVector;

    [Header("Old Position Setup")]
    public Vector3 oldWingCenter;
    public Vector3 oldWingCenterGlobal;
    public Vector3 oldNormalVector;

    [Header("Differences between frames")]
    public Vector3 wingCenterMovement;
    public Vector3 wingCenterMovementGlobal;
    public Vector3 normalVectorDifference;

    private void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
        distanceV = Vector3.Scale(thisCollider.center, transform.lossyScale);
    }

    private void Update()
    {
        oldWingCenter = nWingCenter;
        oldWingCenterGlobal = nWingCenterGlobal;
        oldNormalVector = normalVector;

        nWingCenter = transform.rotation * distanceV;
        nWingCenterGlobal = nWingCenter + transform.position;
        normalVector = transform.up;

        wingCenterMovement = nWingCenter - oldWingCenter;
        wingCenterMovementGlobal = nWingCenterGlobal - oldWingCenterGlobal;
        normalVectorDifference = normalVector - oldNormalVector;

        Debug.DrawLine(nWingCenterGlobal, nWingCenterGlobal + transform.up, Color.red);
    }
}
