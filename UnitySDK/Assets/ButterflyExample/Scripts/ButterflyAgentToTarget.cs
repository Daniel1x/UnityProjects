using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class ButterflyAgentToTarget : Agent
{
    private ButterflyAreaToTarget butterflyArea;
    private RayPerception3D rayPerception;
    private ButterflyController butterflyController;

    private Rigidbody bodyRb;
    public Rigidbody leftWingRb;
    public Rigidbody rightWingRb;
    private ExtendedTransform leftExTransform;
    private ExtendedTransform rightExTransform;
    private Vector3 startPos;
    private Quaternion startRot;
    private Quaternion leftWingStartRot;
    private Quaternion rightWingStartRot;

    private GameObject closestTarget;

    public float maximumAngleOfDeviation = 90f;
    public float positiveRewardMultiplier = 1f;
    public float negativeRewardMultiplier = 1f;

    private void Start()
    {
        butterflyArea = GetComponentInParent<ButterflyAreaToTarget>();
        butterflyController = GetComponent<ButterflyController>();
        bodyRb = GetComponent<Rigidbody>();
        rayPerception = GetComponent<RayPerception3D>();
        GetStartTransform();

        leftExTransform = leftWingRb.gameObject.GetComponent<ExtendedTransform>();
        rightExTransform = rightWingRb.gameObject.GetComponent<ExtendedTransform>();
    }

    private void FixedUpdate()
    {
        FindClosestTarget();
        TryToCatch();
    }

    private void TryToCatch()
    {
        if (closestTarget == null) return;
        RewardAgentRotation();

        if (Vector3.Distance(transform.position, closestTarget.transform.position) < butterflyArea.catchRange)
        {
            CatchTarget(closestTarget);
        }
    }

    private void RewardAgentRotation()
    {
        if (Vector3.Angle(transform.forward, closestTarget.transform.position - transform.position) < maximumAngleOfDeviation)
        {
            AddReward(0.0001f * positiveRewardMultiplier);
        }
        else
        {
            AddReward(-0.0001f * negativeRewardMultiplier);
        }
        if (Vector3.Angle(transform.up, Vector3.up) < maximumAngleOfDeviation)
        {
            AddReward(0.0001f * positiveRewardMultiplier);
        }
        else
        {
            AddReward(-0.0001f * negativeRewardMultiplier);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("target"))
        {
            CatchTarget(collision.gameObject);
        }
        if (collision.transform.CompareTag("wall"))
        {
            AddReward(-0.1f);
        }
    }

    public void AddCollisionReward(string tag, float reward = -0.1f)
    {
        if (tag == "wall")
        {
            AddReward(reward);
        }
    }

    private void CatchTarget(GameObject target)
    {
        butterflyArea.MoveSpecificTarget(target);
        AddReward(1f);
    }

    private void FindClosestTarget()
    {
        if (butterflyArea.targetList != null)
        {
            foreach (GameObject thisTarget in butterflyArea.targetList)
            {
                if (thisTarget != null)
                {
                    closestTarget = thisTarget;
                }
            }
        }
        else
        {
            closestTarget = null;
        }
    }

    private void GetStartTransform()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        leftWingStartRot = leftWingRb.transform.rotation;
        rightWingStartRot = rightWingRb.transform.rotation;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Convert actions to torque values
        Vector3 leftWingTorque = new Vector3(WhichWay(vectorAction[0]), WhichWay(vectorAction[1]), WhichWay(vectorAction[2]));
        Vector3 rightWingTorque = new Vector3(WhichWay(vectorAction[3]), WhichWay(vectorAction[4]), WhichWay(vectorAction[5]));

        //Set controller values
        butterflyController.SetInputs(leftWingTorque, rightWingTorque);

        //Small negative reward every step
        AddReward(-1f / agentParameters.maxStep);
    }

    private float WhichWay(float actionID)
    {
        if (actionID == 1f) return -1f;
        else if (actionID == 2f) return 1f;
        else return 0f;
    }

    public override void AgentReset()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        leftWingRb.transform.rotation = leftWingStartRot;
        rightWingRb.transform.rotation = rightWingStartRot;
        bodyRb.velocity = Vector3.zero;
        bodyRb.angularVelocity = Vector3.zero;
        leftWingRb.velocity = Vector3.zero;
        leftWingRb.angularVelocity = Vector3.zero;
        rightWingRb.velocity = Vector3.zero;
        rightWingRb.angularVelocity = Vector3.zero;

        butterflyArea.ResetArea();
    }

    public override void CollectObservations()
    {
        //Observations
        //Body
        AddVectorObs(transform.forward);
        AddVectorObs(transform.position);
        AddVectorObs(transform.rotation);
        AddVectorObs(bodyRb.velocity);
        AddVectorObs(bodyRb.angularVelocity);
        //Left Wing
        AddVectorObs(leftWingRb.transform.position);
        AddVectorObs(leftWingRb.transform.rotation);
        AddVectorObs(leftWingRb.velocity);
        AddVectorObs(leftWingRb.angularVelocity);
        //Right Wing
        AddVectorObs(rightWingRb.transform.position);
        AddVectorObs(rightWingRb.transform.rotation);
        AddVectorObs(rightWingRb.velocity);
        AddVectorObs(rightWingRb.angularVelocity);
        //Target
        if (closestTarget != null)
        {
            AddVectorObs(closestTarget.transform.position);
            AddVectorObs(Vector3.Distance(transform.position, closestTarget.transform.position));
            AddVectorObs((closestTarget.transform.position - transform.position).normalized);
        }
        else
        {
            AddVectorObs(Vector3.zero);
            AddVectorObs(0f);
            AddVectorObs(Vector3.zero);
        }
        //RayCast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit))
        {
            AddVectorObs(hit.distance);
        }
        else
        {
            AddVectorObs(0f);
        }
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            AddVectorObs(hit.distance);
        }
        else
        {
            AddVectorObs(0f);
        }
        //RayPerception
        //rayDistance
        //rayAngles - list of angles (0 - right, 90 - forward, 180 - left, 270 - back)
        //detectableObjects - list of tags
        //startOffset - offset of rays from transform
        //endOffset - offset of rays from transform
        float rayDistance = 30f;
        float[] rayAngles = new float[12];
        for (int i = 0; i < 12; i++)
        {
            rayAngles[i] = i * 30f;
        }
        string[] detectableObjects = { "wall", "target" };
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

}
