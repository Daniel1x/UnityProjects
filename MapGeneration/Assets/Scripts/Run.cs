using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Run : MonoBehaviour
{
    public float power = 1f;
    private Rigidbody rb;
    private Vector3 offset = new Vector3(0f, 2f, -10f);

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.AddForce(Time.deltaTime * power * new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), ForceMode.Impulse);
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = transform.position + offset;
        Camera.main.transform.LookAt(transform);
    }
}
