using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public GameObject leftWingGO;
    public GameObject rightWingGO;

    Rigidbody leftWingRb;
    Rigidbody rightWingRb;

    ExtendedTransform leftExT;
    ExtendedTransform rightExT;

    public float rightInput;
    public float forwardInput;
    public float upwardInput;

    public Vector3 leftWingTorque;
    public Vector3 rightWingTorque;

    public Vector3 lTorqueRad;
    public Vector3 lTorqueDeg;
    public Vector3 rTorqueRad;
    public Vector3 rTorqueDeg;

    public float forceMultiplier = 1000f;

    private Vector3 leftWingOffset;
    private Vector3 rightWingOffset;

    public int numFixes = 0;

    private void Start()
    {
        leftWingRb = leftWingGO.GetComponent<Rigidbody>();
        rightWingRb = rightWingGO.GetComponent<Rigidbody>();

        leftExT = leftWingGO.GetComponent<ExtendedTransform>();
        rightExT = rightWingGO.GetComponent<ExtendedTransform>();

        leftWingRb.maxAngularVelocity = 10f;
        rightWingRb.maxAngularVelocity = 10f;

        leftWingOffset = leftWingGO.transform.position - transform.position;
        rightWingOffset = rightWingGO.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        FixWings();

        rightInput = Input.GetAxis("Right");
        forwardInput = Input.GetAxis("Forward");
        upwardInput = Input.GetAxis("Upward");

        leftWingTorque = forceMultiplier * leftWingRb.mass * Time.fixedDeltaTime * (new Vector3(rightInput, -upwardInput, -forwardInput));
        rightWingTorque = forceMultiplier * rightWingRb.mass * Time.fixedDeltaTime * (new Vector3(rightInput, upwardInput, forwardInput));

        leftWingRb.AddRelativeTorque(leftWingTorque);
        rightWingRb.AddRelativeTorque(rightWingTorque);

        lTorqueRad = leftWingRb.angularVelocity;
        lTorqueDeg = lTorqueRad * Mathf.Rad2Deg;
        rTorqueRad = rightWingRb.angularVelocity;
        rTorqueDeg = rTorqueRad * Mathf.Rad2Deg;

        Debug.DrawLine(leftExT.centerGlobal, leftExT.centerGlobal + leftWingTorque * forceMultiplier, Color.green);
        Debug.DrawLine(rightExT.centerGlobal, rightExT.centerGlobal + rightWingTorque * forceMultiplier, Color.green);

        LockWings();
    }

    private void LockWings()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            leftWingRb.freezeRotation = true;
            rightWingRb.freezeRotation = true;
        }
        else
        {
            leftWingRb.freezeRotation = false;
            rightWingRb.freezeRotation = false;
        }
    }

    private void FixWings()
    {
        if (Vector3.Distance(transform.position, leftWingGO.transform.position) > 1.5f * leftWingOffset.magnitude)
        {
            Vector3 leftWingPos = transform.position + transform.rotation * leftWingOffset;
            leftWingGO.transform.position = leftWingPos;
            Debug.Log("Left wing has been fixed!");
            leftWingRb.velocity = leftWingRb.velocity.normalized;
            numFixes++;
        }
        if (Vector3.Distance(transform.position, rightWingGO.transform.position) > 1.5f * rightWingOffset.magnitude)
        {
            Vector3 rightWingPos = transform.position + transform.rotation * rightWingOffset;
            rightWingGO.transform.position = rightWingPos;
            Debug.Log("Right wing has been fixed!");
            rightWingRb.velocity = rightWingRb.velocity.normalized;
            numFixes++;
        }
    }
}
