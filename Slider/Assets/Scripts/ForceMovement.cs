using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceMovement : MonoBehaviour
{
    public int choise = 1;

    [SerializeField]
    string _movementX;

    [SerializeField]
    string _movementZ;

    [SerializeField]
    float _speed;

    Rigidbody _rigidBody;
    float _moveX;
    float _moveZ;

    void Start()
    {
        _rigidBody = this.GetComponent<Rigidbody>();

        if (_rigidBody == null)
        {
            Debug.LogError("Rigid body could not be found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _moveX = Input.GetAxis(_movementX);
        _moveZ = Input.GetAxis(_movementZ);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            choise++;
            if (choise > 4) choise = 1;
        }
        if (_rigidBody != null)
        {
            Vector3 moveVector = new Vector3(_moveX, 0, _moveZ) * _speed;
            //Vector3 pos = new Vector3(_rigidBody.position.x, _rigidBody.position.y, _rigidBody.position.z);
            //Quaternion rot = _rigidBody.transform.rotation;
            //Vector3 powVect = new Vector3() * _speed;
            Debug.Log(_moveX);
            if (choise == 1) _rigidBody.AddForce(moveVector, ForceMode.Acceleration);
            else if (choise == 2) _rigidBody.AddForce(moveVector, ForceMode.Force);
            else if (choise == 3) _rigidBody.AddForce(moveVector, ForceMode.Impulse);
            else if (choise >= 4) _rigidBody.AddForce(moveVector, ForceMode.VelocityChange);
        }
    }

}