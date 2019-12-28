using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    string movementX;
    [SerializeField]
    string movementZ;
    [SerializeField]
    float speed;
    [SerializeField]
    float maxVelocity;
    float moveX;
    float moveZ;
    bool Mode;
    Rigidbody rb = new Rigidbody();
    Rigidbody rigidbody = new Rigidbody();
    
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        maxVelocity = 100f;
        Mode = true;
    }
    
    private void Update()
    {
        OutOfMap();

        moveX = Input.GetAxis(movementX);
        moveZ = Input.GetAxis(movementZ);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Mode = !Mode;
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = new Vector3(moveX, 0, moveZ) * speed;

        if (Mode)
        {
            rb.AddForce(moveVector, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveVector, ForceMode.VelocityChange);
        }

        if (Velocity(rb) > maxVelocity)
        {
            rigidbody = rb;
            rigidbody.velocity = rb.velocity.normalized * maxVelocity;
            rb.velocity = rigidbody.velocity;
        }
    }

    private float Velocity(Rigidbody rigidbody)
    {
        return rigidbody.velocity.magnitude;
    }

    private void OutOfMap(float maxDistance = 22.5f)
    {
        if(transform.position.x>maxDistance || transform.position.x < -maxDistance)
        {
            transform.position = transform.position - new Vector3(transform.position.x, 0, 0);
        }
    }
}
