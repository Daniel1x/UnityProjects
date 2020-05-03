using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class responsible for moving the player.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] [Range(0f, 360f)] private float rotationSpeed = 90f;
    [SerializeField] [Range(0f, 50f)] private float movementSpeed = 10f;
    [SerializeField] [Range(0f, 1000f)] private float jumpForce = 500f;
    [SerializeField] [Range(-100f, -9.81f)] private float gravityForce = -10f;
    [SerializeField] [Range(0.5f, 10f)] private float rayRange = 1f;
    [SerializeField] [Range(1, 360)] private int numRays = 90;
    [SerializeField] [Range(1f, 90f)] private float maxAngle = 45f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0f, gravityForce, 0f);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (Input.anyKey)
        {
            float dt = Time.deltaTime;
            Rotate(dt);
            Translate(dt);
            Jump();
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }

    private void Translate(float dt)
    {
        RaycastHit hit;
        for(int i = 0; i < numRays; i++)
        {
            Quaternion angle = Quaternion.Euler(0f, -maxAngle + (2 * i * maxAngle / numRays), 0f);
            if (Physics.Raycast(transform.position, angle * transform.forward, out hit, rayRange))
            {
                return;
            }
        }

        float movement = Input.GetAxis("Vertical") * dt * movementSpeed;
        transform.Translate(Vector3.forward * movement);
    }

    private void Rotate(float dt)
    {
        float rotation = Input.GetAxis("Horizontal") * dt * rotationSpeed;
        transform.Rotate(new Vector3(0f, rotation, 0f));
    }
}
