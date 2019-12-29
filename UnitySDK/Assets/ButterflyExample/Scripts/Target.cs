using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float targetSpeed;

    private float randomizedSpeed = 0f;
    private float nextActionTime = -1f;
    private Vector3 nextPosition;
    private ButterflyArea butterflyArea;

    private void Start()
    {
        butterflyArea = GetComponentInParent<ButterflyArea>();
    }

    private void FixedUpdate()
    {
        if (targetSpeed > 0f  && !butterflyArea.spawnStraight)
        {
            Move();
        }
    }

    private void Move()
    {
        if (Time.fixedTime >= nextActionTime)
        {
            randomizedSpeed = targetSpeed * UnityEngine.Random.Range(0.25f, 1.25f);
            nextPosition = ButterflyArea.ChooseRandomPosition(transform.parent.position, 0f, 360f, butterflyArea.butterflySpawnRange + 1f, butterflyArea.targetSpawnRange + 1f);
            transform.rotation = Quaternion.LookRotation(nextPosition - transform.position, new Vector3(1f, 1f, 1f));
            float timeOfMovement = Vector3.Distance(transform.position, nextPosition) / randomizedSpeed;
            nextActionTime = Time.fixedTime + timeOfMovement;
        }
        else
        {
            Vector3 moveVector = randomizedSpeed * transform.forward * Time.fixedDeltaTime;
            if (moveVector.magnitude <= Vector3.Distance(transform.position, nextPosition))
            {
                transform.position += moveVector;
            }
            else
            {
                transform.position = nextPosition;
                nextActionTime = Time.fixedTime;
            }
        }
    }
}
