using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    private float distance;
    public float power = 1.0f;
    private float border = 1000.0f;


    void Start()
    {
        distance = Vector3.Distance(gameObject.transform.position, Vector3.zero);
    }
    
    void Update()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
        distance = Vector3.Distance(gameObject.transform.position, Vector3.zero);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (distance > border)
        {
            
            rigidbody.AddForce(-gameObject.transform.position *rigidbody.mass* Time.deltaTime,ForceMode.Force);
            rigidbody.drag = 1.0f;
        }
        else if (gameObject.tag == "Player")
        {
            rigidbody.drag = 0.1f;
        } 
        else
        {
            rigidbody.drag = 0.0f;
        }
    }
}
