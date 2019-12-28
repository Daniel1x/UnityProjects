using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float movementSpeed = 50f;
    [SerializeField] float turnSpeed = 200f;
    Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
    }
    
    void Update ()
    {
        Turn();
        Trust();
	}

    void Turn()
    {
        float horiz = turnSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        float vertic = turnSpeed * Time.deltaTime * Input.GetAxis("Pitch");
        float roll = turnSpeed * Time.deltaTime * Input.GetAxis("Roll");

        myTransform.Rotate(vertic,horiz,roll);
    }

    void Trust()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
           myTransform.position += myTransform.forward * movementSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        }
    }

    private Vector3 RandVect3()
    {
        Vector3 RndV = new Vector3((2 * Random.value) - 1, (2 * Random.value) - 1, (2 * Random.value) - 1);
        return RndV;
    }
}
