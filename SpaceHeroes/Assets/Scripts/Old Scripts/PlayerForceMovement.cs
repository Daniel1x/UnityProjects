using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerForceMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 50f;
    [SerializeField] float turnSpeed = 200f;
    Transform myTransform;
    Rigidbody rigidBody;
    public int choise = 1;
    //private float speed=50;
    public float moveX;
    public float moveZ;

    private void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
            Debug.LogError("Rigid body could not be found.");
        }
    }

    void Update()
    {
        moveX = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Horizontal");
        Turn();
        Trust();
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.isKinematic = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rigidBody.isKinematic = false;
        }
    }
    /*
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            choise++;
            if (choise > 4) choise = 1;
        }
        if (rigidBody != null)
        {
            Vector3 moveVector = new Vector3(moveX, 0, moveZ) * speed;
            //Vector3 pos = new Vector3(_rigidBody.position.x, _rigidBody.position.y, _rigidBody.position.z);
            //Quaternion rot = _rigidBody.transform.rotation;
            //Vector3 powVect = new Vector3() * _speed;
            Debug.Log(moveX);
            if (choise == 1) rigidBody.AddForce(moveVector, ForceMode.Acceleration);
            else if (choise == 2) rigidBody.AddForce(moveVector, ForceMode.Force);
            else if (choise == 3) rigidBody.AddForce(moveVector, ForceMode.Impulse);
            else if (choise >= 4) rigidBody.AddForce(moveVector, ForceMode.VelocityChange);
            
        }
    }*/

    void Turn()
    {
        float yaw = turnSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        float pitch = turnSpeed * Time.deltaTime * Input.GetAxis("Pitch");
        float roll = turnSpeed * Time.deltaTime * Input.GetAxis("Roll");

        myTransform.Rotate(pitch, yaw, roll);
    }
    void Trust()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            myTransform.position += myTransform.forward * movementSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        }
    }
}