using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    [SerializeField] GameObject[] wings = new GameObject[4];
    Wing[] wing = new Wing[4];

    [SerializeField] float forceRatio = 1f;
    Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetWings();
    }

    private void Update()
    {
        ApplyForceCreatedByWings();
    }
    
    private void ApplyForceCreatedByWings()
    {
        for(int i = 0; i < wings.Length; i++)
        {
            if (true)
            {
                rb.AddForceAtPosition(forceRatio * wing[i].force, wings[i].transform.position, ForceMode.Force);
                Debug.DrawLine(wings[i].transform.position, wings[i].transform.position + wing[i].force * forceRatio, Color.cyan);
            }
            //else
            //{
            //    rb.AddForce(forceRatio * wing[i].force, ForceMode.Force);
            //    Debug.DrawLine(transform.position, transform.position + wing[i].force * forceRatio, Color.cyan);
            //}
        }
    }

    private void GetWings()
    {
        for (int i = 0; i < wings.Length; i++)
            wing[i] = wings[i].GetComponent<Wing>();
    }
}
