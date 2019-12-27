using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    public float multiplier = 1;

    public Transform head;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * Time.deltaTime * multiplier, head.position, ForceMode.Impulse);
        }
    }
}
