using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyController : MonoBehaviour
{
    [Header("Wings GameObjects")]
    public GameObject leftWingGO;
    public GameObject rightWingGO;

    Rigidbody leftWingRb;
    Rigidbody rightWingRb;
    Rigidbody bodyRb;

    ExtendedTransform leftExT;
    ExtendedTransform rightExT;
    [Header("Control Values Of Left Wing")]
    public float rightAxisInputValueLW;
    public float forwardAxisInputValueLW;
    public float upwardAxisInputValueLW;
    [Header("Control Values Of Right Wing")]
    public float rightAxisInputValueRW;
    public float forwardAxisInputValueRW;
    public float upwardAxisInputValueRW;
    [Header("Control Torque")]
    public Vector3 leftWingTorque;
    public Vector3 rightWingTorque;
    [Header("Force Multiplier")]
    public float forceMultiplier = 1000f;

    private Vector3 leftWingOffset;
    private Vector3 rightWingOffset;
    [Header("Limiting Values")]
    public float maxVelocityMagnitude = 5f;
    public float maxAngVelocityMagnitude = 5f;

    private int numFixes = 0;

    public bool controlledByPlayer = false;

    private void Start()
    {
        leftWingRb = leftWingGO.GetComponent<Rigidbody>();
        rightWingRb = rightWingGO.GetComponent<Rigidbody>();
        bodyRb = GetComponent<Rigidbody>();

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
        GetInputs();
        CalculateTorque();
        AddTorque();
        ShowVectors();
        LockWings();
    }

    private void AddTorque()
    {
        leftWingRb.AddRelativeTorque(leftWingTorque);
        rightWingRb.AddRelativeTorque(rightWingTorque);
    }

    private void CalculateTorque()
    {
        leftWingTorque = forceMultiplier * leftWingRb.mass * Time.fixedDeltaTime * new Vector3(rightAxisInputValueLW, -upwardAxisInputValueLW, -forwardAxisInputValueLW);
        rightWingTorque = forceMultiplier * rightWingRb.mass * Time.fixedDeltaTime * new Vector3(rightAxisInputValueRW, upwardAxisInputValueRW, forwardAxisInputValueRW);
    }

    private void ShowVectors()
    {
        if (leftExT.debugEnabled) Debug.DrawLine(leftExT.centerGlobal, leftExT.centerGlobal + leftWingTorque * forceMultiplier, Color.green);
        if (rightExT.debugEnabled) Debug.DrawLine(rightExT.centerGlobal, rightExT.centerGlobal + rightWingTorque * forceMultiplier, Color.green);
    }

    private void GetInputs()
    {
        if (!controlledByPlayer) return;
        rightAxisInputValueLW = Input.GetAxis("Right");
        forwardAxisInputValueLW = Input.GetAxis("Forward");
        upwardAxisInputValueLW = Input.GetAxis("Upward");
        rightAxisInputValueRW = rightAxisInputValueLW;
        forwardAxisInputValueRW = forwardAxisInputValueLW;
        upwardAxisInputValueRW = upwardAxisInputValueLW;
    }

    public void SetInputs(Vector3 leftWingTorqueVector, Vector3 rightWingTorqueVector)
    {
        if (controlledByPlayer) return;
        rightAxisInputValueLW = leftWingTorqueVector.x;
        forwardAxisInputValueLW = leftWingTorqueVector.y;
        upwardAxisInputValueLW = leftWingTorqueVector.z;
        rightAxisInputValueRW = rightWingTorqueVector.x;
        forwardAxisInputValueRW = rightWingTorqueVector.y;
        upwardAxisInputValueRW = rightWingTorqueVector.z;
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
        if (Vector3.Distance(transform.position, leftWingGO.transform.position) > 2f * leftWingOffset.magnitude)
        {
            Vector3 leftWingPos = transform.position + transform.rotation * leftWingOffset;
            leftWingGO.transform.position = leftWingPos;
            Debug.Log("Left wing has been fixed!");
            if (leftWingRb.velocity.magnitude > maxVelocityMagnitude) leftWingRb.velocity = leftWingRb.velocity.normalized * maxVelocityMagnitude;
            if (leftWingRb.angularVelocity.magnitude > maxAngVelocityMagnitude) leftWingRb.angularVelocity = leftWingRb.angularVelocity.normalized * maxAngVelocityMagnitude;
            numFixes++;
        }
        if (Vector3.Distance(transform.position, rightWingGO.transform.position) > 2f * rightWingOffset.magnitude)
        {
            Vector3 rightWingPos = transform.position + transform.rotation * rightWingOffset;
            rightWingGO.transform.position = rightWingPos;
            Debug.Log("Right wing has been fixed!");
            if (rightWingRb.velocity.magnitude > maxVelocityMagnitude) rightWingRb.velocity = rightWingRb.velocity.normalized * maxVelocityMagnitude;
            if (rightWingRb.angularVelocity.magnitude > maxAngVelocityMagnitude) rightWingRb.angularVelocity = rightWingRb.angularVelocity.normalized * maxAngVelocityMagnitude;
            numFixes++;
        }
        if (bodyRb.velocity.magnitude > maxVelocityMagnitude) bodyRb.velocity = bodyRb.velocity.normalized * maxVelocityMagnitude;
        if (bodyRb.angularVelocity.magnitude > maxAngVelocityMagnitude) bodyRb.angularVelocity = bodyRb.angularVelocity.normalized * maxAngVelocityMagnitude;
    }
}
