using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class ButterflyAgent : Agent
{
    private ButterflyArea butterflyArea;
    private RayPerception3D rayPerception;
    private ButterflyController butterflyController;

    private Rigidbody bodyRb;
    public Rigidbody leftWingRb;
    public Rigidbody rightWingRb;
    private Vector3 startPos;
    private Quaternion startRot;
    private Quaternion leftWingStartRot;
    private Quaternion rightWingStartRot;

    private GameObject closestTarget;

    private void Start()
    {
        butterflyArea = GetComponentInParent<ButterflyArea>();
        butterflyController = GetComponent<ButterflyController>();
        bodyRb = GetComponent<Rigidbody>();
        rayPerception = GetComponent<RayPerception3D>();
        GetStartTransform();
    }

    private void FixedUpdate()
    {
        FindClosestTarget();
        TryToCatch();
    }

    private void TryToCatch()
    {
        if (closestTarget == null) return;
        if (Vector3.Distance(transform.position, closestTarget.transform.position) < butterflyArea.catchRange)
        {
            CatchTarget(closestTarget);
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

    private void CatchTarget(GameObject target)
    {
        butterflyArea.RemoveSpecificTarget(target);
        AddReward(1f);
    }

    private void FindClosestTarget()
    {
        if (butterflyArea.targetList != null)
        { 
            float maxDistance = Mathf.Infinity;
            foreach (GameObject thisTarget in butterflyArea.targetList)
            {
                if (thisTarget != null)
                {
                    if (Vector3.Distance(transform.position, thisTarget.transform.position) < maxDistance)
                    {
                        maxDistance = Vector3.Distance(transform.position, thisTarget.transform.position);
                        closestTarget = thisTarget;
                    }
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

    //Funkcja odpowiadająca za wykonywanie akcji na podstawie parametrów przesłanych w wektorze akcji.
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

    //Funkcja przywracająca danego agenta do ustawień początkowych.
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

    //Funkcja gromadząca informacje o aktualnym stanie.
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
        if (Physics.Raycast(transform.position + 2 * Vector3.up, Vector3.up, out hit))
        {
            AddVectorObs(hit.distance);
        }
        else
        {
            AddVectorObs(0f);
        }
        if(Physics.Raycast(transform.position - 2*Vector3.up,-Vector3.up,out hit))
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
        float[] rayAngles = new float[36];
        for (int i = 0; i < 36; i++)
        {
            rayAngles[i] = i * 10f;
        }
        string[] detectableObjects = { "wall", "target" };
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

}
